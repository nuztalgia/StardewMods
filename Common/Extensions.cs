using System;
using System.Collections.Generic;

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
