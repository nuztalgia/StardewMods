using System.Collections.Immutable;

namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal static class ContentPatcherBridge {

  private static readonly HashSet<string> ManagedModIds = new();

  private static IDictionary<string, IEnumerable<object>>? ManagedConfigs;
  private static IDictionary<string, object?>? AllRawConfigs;

  private static Action? OnContentPatcherInitialized;

  internal static bool HasMissingMethods => MissingMethodText != string.Empty;
  internal static string MissingMethodText { get; private set; } = string.Empty;

  internal static void Initialize(string modId, Action onContentPatcherInitialized) {
    MethodInfo? readConfigFile = GetContentPatcherMethod("Framework.ConfigFileHandler:Read");
    MethodInfo? initializeMod = GetContentPatcherMethod("ModEntry:Initialize");

    if ((readConfigFile == null) || (initializeMod == null)) {
      return;
    }

    AllRawConfigs = new Dictionary<string, object?>();
    OnContentPatcherInitialized = onContentPatcherInitialized;

    HarmonyPatcher harmonyPatcher = new(modId);
    ApplyPostfix(readConfigFile, nameof(ConfigFileHandler_Read_Postfix));
    ApplyPostfix(initializeMod, nameof(ModEntry_Initialize_Postfix));

    void ApplyPostfix(MethodInfo originalMethod, string patchMethodName) {
      harmonyPatcher.ForMethod(originalMethod)
          .ApplyPostfixPatch(typeof(ContentPatcherBridge), patchMethodName);
    }

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

    static IEnumerable<object> ParseModConfig(string contentPackId, object rawConfigDict) {
      Log.Trace($"Parsing config for registered content pack '{contentPackId}'.");
      // TODO: Implement this.
      return Array.Empty<object>();
    }
  }
}
