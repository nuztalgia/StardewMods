using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class Globals {

  private readonly static Dictionary<string, string> I18nCache = new();

  internal static string GetI18nString(string name) {
    if (!I18nCache.ContainsKey(name)
        && (typeof(I18n.Keys).GetField(name)?.GetValue(null) is string key)) {
      I18nCache[name] = I18n.GetByKey(key);
    }
    return I18nCache[name];
  }
}
