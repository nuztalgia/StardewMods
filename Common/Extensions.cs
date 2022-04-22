using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common;

internal static class StringExtensions {

  internal static bool IsEmpty(this string s) {
    return s.Length == 0;
  }

  internal static string Format(this string s, params object[] args) {
    return (args.Length == 0) ? s : string.Format(s, args);
  }

  internal static string CapitalizeFirstChar(this string s) {
    return s.IsEmpty() ? string.Empty : string.Concat(s[0].ToString().ToUpper(), s.AsSpan(1));
  }

  internal static string[] CommaSplit(this string s) {
    return s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
  }
}

internal static class IEnumerableExtensions {

  internal static string CommaJoin(this IEnumerable<object> items) {
    return string.Join(", ", items);
  }
}

internal static class ModRegistryExtensions {

  // Inspired by:  "This is really bad. Pathos don't kill me."  - kittycatcasey
  internal static bool TryFetchMod<T>(
      this IModRegistry modRegistry,
      string modId,
      [NotNullWhen(true)] out T? mod) {

    string modType = typeof(T).Name switch {
      nameof(IMod) => "Mod",
      nameof(IContentPack) => "ContentPack",
      _ => throw new ArgumentException($"Unsupported type for mod registry: '{typeof(T).Name}'."),
    };

    IModInfo? modInfo = modRegistry.Get(modId);
    mod = (modInfo?.GetType().GetProperty(modType)?.GetValue(modInfo) is T value) ? value : default;
    return mod is not null;
  }
}
