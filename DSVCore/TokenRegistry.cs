using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class TokenRegistry {

  private static IDictionary<string, Func<IEnumerable<string>>> TokenData =
      new Dictionary<string, Func<IEnumerable<string>>>();

  internal static IDictionary<string, Func<IEnumerable<string>>> GetData() {
    // Freeze the data when it's requested, so that we know if we're adding any tokens too late.
    if (TokenData is not ImmutableSortedDictionary<string, Func<IEnumerable<string>>>) {
      Log.Trace($"Closing token registration. A total of {TokenData.Count} tokens are registered.");
      TokenData = TokenData.ToImmutableSortedDictionary();
    }
    return TokenData;
  }

  internal static void AddBoolToken(string tokenName, Func<bool> getValue,
      string? valueIfTrue = null, string? valueIfFalse = null, string? autoValueString = null) {
    if (!string.IsNullOrEmpty(autoValueString)) {
      valueIfTrue = autoValueString;
      valueIfFalse = "No" + autoValueString;
    } else if (string.IsNullOrEmpty(valueIfTrue) || string.IsNullOrEmpty(valueIfFalse)) {
      valueIfTrue = true.ToString();
      valueIfFalse = false.ToString();
    }
    Log.Verbose($"Adding token '{tokenName}' with possible values " +
                $"'{valueIfTrue}' or '{valueIfFalse}'.");
    AddToken(tokenName, () => new[] { getValue() ? valueIfTrue : valueIfFalse });
  }

  internal static void AddIntToken(string tokenName, Func<int> getValue,
      int minValue = int.MinValue, int maxValue = int.MaxValue) {
    Log.Verbose($"Adding token '{tokenName}' with possible values " +
                $"between {minValue} and {maxValue}.");
    AddToken(tokenName, () => new[] { Math.Clamp(getValue(), minValue, maxValue).ToString() });
  }

  // Note: This method is currently unused, but might be needed in the future.
  internal static void AddStringToken(string tokenName, Func<string> getValue) {
    Log.Verbose($"Adding token '{tokenName}' with any possible string value.");
    AddToken(tokenName, () => new[] { getValue() });
  }

  internal static void AddEnumToken<T>(string tokenName, Func<object?> getValue) where T : Enum {
    Log.Verbose($"Adding token '{tokenName}' with one of the following possible values: " +
                $"'{string.Join(", ", typeof(T).GetEnumNames())}'.");
    AddToken(tokenName, () => new[] { (getValue() is T value) ? value.ToString() : "" });
  }

  internal static void AddCompositeToken(string tokenName, Dictionary<string, Func<bool>> entries) {
    Log.Verbose($"Adding token '{tokenName}' with zero, one, or multiple of the following " +
                $"possible values: '{string.Join(", ", entries.Keys)}'.");
    AddToken(tokenName, () => {
      IEnumerable<string> values = entries.Where(entry => entry.Value()).Select(entry => entry.Key);
      return values.Any() ? values : new[] { "" };
    });
  }

  private static void AddToken(string tokenName, Func<IEnumerable<string>> getValue) {
    if (string.IsNullOrEmpty(tokenName)) {
      Log.Error($"Cannot register token with a null/empty name.");
    } else if (!TokenData.TryAdd(tokenName, getValue)) { // This line is where the token is added.
      Log.Error($"Cannot register token named '{tokenName}'. That name is already taken.");
    }
  }
}
