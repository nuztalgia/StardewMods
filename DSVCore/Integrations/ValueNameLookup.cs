using System;
using System.Collections.Generic;
using System.Reflection;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ValueNameLookup {

  private static readonly Dictionary<string, string> ValueNameCache = new();

  internal static string GetValueDisplayName(string valueName) {
    if (Globals.GetI18nString($"Value_{valueName}") is string valueDisplayName) {
      // Store the original value name in the cache to allow "reverse lookup" when needed later.
      ValueNameCache[valueDisplayName] = valueName;
      return valueDisplayName;
    } else {
      Log.Trace($"Value name '{valueName}' does not have a defined display name. Returning as-is.");
      return valueName;
    }
  }

  internal static string GetValueName(string valueDisplayName) {
    if (ValueNameCache.TryGetValue(valueDisplayName, out string? valueName)) {
      return valueName;
    } else {
      Log.Trace($"Value display name '{valueDisplayName}' is not in the cache. Returning as-is.");
      return valueDisplayName;
    }
  }

  internal static IEnumerable<string> GetAllValueDisplayNames(PropertyInfo property) {
    foreach (string valueName in Enum.GetNames(property.PropertyType)) {
      yield return GetValueDisplayName(valueName);
    }
  }

  // Extension method for PropertyInfo.
  internal static string GetValueDisplayName(this PropertyInfo property, object obj) {
    return (property.GetValue(obj)?.ToString() is string valueName)
           ? GetValueDisplayName(valueName)
           : string.Empty;
  }

  // Extension method for PropertyInfo.
  internal static void SetValueByDisplayName(
      this PropertyInfo property, object obj, string valueDisplayName) {
    property.SetValue(obj, Enum.Parse(property.PropertyType, GetValueName(valueDisplayName)));
  }
}
