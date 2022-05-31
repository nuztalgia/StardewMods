using System.Collections.Immutable;

namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal static class ContentPatcherBridge {

  private static readonly Dictionary<string, MethodInfo?> MethodCache = new();
  private static readonly HashSet<string> ManagedModIds = new();

  private static IDictionary<string, IEnumerable<ConfigFieldData>>? ManagedConfigs;
  private static IDictionary<string, object?>? AllRawConfigs;

  private static Action? OnContentPatcherInitialized;

  internal static bool HasMissingMethods => MissingMethodText != string.Empty;
  internal static string MissingMethodText { get; private set; } = string.Empty;

  internal static void Initialize(string modId, Action onContentPatcherInitialized) {
    MethodInfo? readConfig = GetContentPatcherMethod("Framework.ConfigFileHandler:Read");
    MethodInfo? initializeMod = GetContentPatcherMethod("ModEntry:Initialize");

    if ((readConfig == null) || (initializeMod == null)) {
      return;
    }

    HarmonyPatcher harmonyPatcher = new(modId, typeof(ContentPatcherBridge));
    harmonyPatcher.ForMethod(readConfig).ApplyPostfixPatch(nameof(ConfigFileHandler_Read_Postfix));
    harmonyPatcher.ForMethod(initializeMod).ApplyPostfixPatch(nameof(ModEntry_Initialize_Postfix));

    AllRawConfigs = new Dictionary<string, object?>();
    OnContentPatcherInitialized = onContentPatcherInitialized;

    static MethodInfo? GetContentPatcherMethod(string methodName) {
      string qualifiedMethodName = $"ContentPatcher.{methodName}";
      MethodInfo? method = AccessTools.Method(qualifiedMethodName);
      if (method == null) {
        MissingMethodText += $"\n  - Method Not Found: '{qualifiedMethodName}'";
      }
      return method;
    }
  }

  internal static void UpdateManagedMods(IEnumerable<string> modIds) {
    ManagedModIds.Clear();
    ManagedModIds.UnionWith(modIds);
  }

  internal static IEnumerable<ConfigFieldData> GetModConfig(string modId) {
    return ((ManagedConfigs != null)
        && ManagedConfigs.TryGetValue(modId, out IEnumerable<ConfigFieldData>? modConfig))
            ? modConfig
            : Array.Empty<ConfigFieldData>();
  }

  private static void ConfigFileHandler_Read_Postfix(IContentPack contentPack, object __result) {
    if (AllRawConfigs != null) {
      string contentPackId = contentPack.Manifest.UniqueID;
      Log.Verbose($"Tracking config for CP content pack '{contentPackId}'.");
      AllRawConfigs.Add(contentPackId, __result);
    }
  }

  private static void ModEntry_Initialize_Postfix() {
    if ((ManagedConfigs == null) && (AllRawConfigs != null)) {
      ManagedConfigs =
          AllRawConfigs
              .Where(kvp => ManagedModIds.Contains(kvp.Key) && (kvp.Value != null))
              .ToImmutableDictionary(kvp => kvp.Key, kvp => ParseModConfig(kvp.Key, kvp.Value!));
      AllRawConfigs = null;
      OnContentPatcherInitialized?.Invoke();
    }

    static IEnumerable<ConfigFieldData> ParseModConfig(string contentPackId, object rawConfigDict) {
      Log.Trace($"Parsing config for registered content pack '{contentPackId}'.");
      List<ConfigFieldData> configData = new();

      if (TryInvokeMethod(rawConfigDict, "get_Keys") is IEnumerable<string> configKeys) {
        foreach (string configKey in configKeys) {
          object?[] args = new object?[] { configKey, null };
          if ((TryInvokeMethod(rawConfigDict, "TryGetValue", args) is true)
              && (args[1] is object rawConfigField)) {
            configData.Add(ParseConfigField(configKey, rawConfigField));
          }
        }
      }
      return configData;
    }

    static ConfigFieldData ParseConfigField(string configKey, object rawConfigField) {
      (int? numericRangeMin, int? numericRangeMax) = (null, null);
      object?[] args = new object?[] { null, null };

      if (GetBoolean("IsNumericRange", args)) {
        (numericRangeMin, numericRangeMax) = ((int?) args[0], (int?) args[1]);
      }

      return new ConfigFieldData(
          key: configKey,
          currentValues: GetEnumerable("get_Value"),
          defaultValues: GetEnumerable("get_DefaultValues"),
          allowedValues: GetEnumerable("get_AllowValues"),
          allowBlankValues: GetBoolean("get_AllowBlank"),
          allowMultipleValues: GetBoolean("get_AllowMultiple"),
          isBooleanValue: GetBoolean("IsBoolean"),
          numericRangeMin: numericRangeMin,
          numericRangeMax: numericRangeMax);

      IEnumerable<string>? GetEnumerable(string methodName) {
        return TryInvokeMethod(rawConfigField, methodName) as IEnumerable<string>;
      }

      bool GetBoolean(string methodName, object?[]? args = null) {
        return TryInvokeMethod(rawConfigField, methodName, args) is true;
      }
    }
  }

  private static object? TryInvokeMethod(object obj, string methodName, object?[]? args = null) {
    MethodCache.TryGetValue(methodName, out MethodInfo? method);

    if (method == null) {
      Log.Verbose($"Looking for method '{methodName}' on type '{obj.GetType().Name}'.");
      method = MethodCache[methodName] = AccessTools.Method(obj.GetType(), methodName);
    }

    try {
      return method!.Invoke(obj, args ?? Array.Empty<object>());
    } catch (Exception exception) {
      Log.Error($"Unable to invoke method '{methodName}' on object of type " +
          $"'{obj.GetType().AssemblyQualifiedName}'. Technical details:\n{exception}");
      return null;
    }
  }
}
