using System;

namespace Nuztalgia.StardewMods.Common;

internal static class StringExtensions {

  internal static bool IsEmpty(this string s) {
    return s.Length == 0;
  }

  internal static string Format(this string s, params object[] args) {
    return string.Format(s, args);
  }

  internal static string CapitalizeFirstChar(this string s) {
    return s.IsEmpty() ? string.Empty : string.Concat(s[0].ToString().ToUpper(), s.AsSpan(1));
  }
}
