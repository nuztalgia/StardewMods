namespace Nuztalgia.StardewMods.Common;

internal static class I18nHelper {

  private const string I18nTypeName = "MTCB.I18n";
  private const string InitMethodName = "Init";

#if I18N_KEYS
  private const string GetByKeyMethodName = "GetByKey";
  private const string KeysNestedTypeName = "Keys";

  private static readonly Dictionary<string, string> KeyCache = new();

#pragma warning disable CS8618 // Non-nullable field must contain non-null value. Set in Initialize.
  private static Func<string, string> FetchStringByKeyName;
#pragma warning restore CS8618
#endif

  internal static void Initialize(ITranslationHelper translationHelper) {
    Type i18n = Type.GetType(I18nTypeName)!;
    i18n.GetMethod(InitMethodName)!.Invoke(null, new[] { translationHelper });

#if I18N_KEYS
    Type keysNestedType = i18n.GetNestedType(KeysNestedTypeName)!;
    MethodInfo getByKeyMethod = i18n.GetMethod(GetByKeyMethodName)!;

    FetchStringByKeyName = (string keyName) => FetchString(keyName);

    string FetchString(string keyName) {
      return ((keysNestedType.GetField(keyName)?.GetValue(null) is string key)
          && (getByKeyMethod.Invoke(null, new object?[] { key, null }) is Translation value))
              ? value.ToString() : keyName;
    }
#endif
  }

#if I18N_KEYS
  internal static string GetStringByKeyName(string keyName) {
    if (!KeyCache.ContainsKey(keyName)) {
      KeyCache.Add(keyName, FetchStringByKeyName(keyName));
    }
    return KeyCache[keyName];
  }
#endif
}
