using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ContentPatcher;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common.ContentPatcher;

internal sealed class Integration : BaseIntegration<IContentPatcherAPI> {

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private Integration(IContentPatcherAPI api, IManifest manifest) : base(api, manifest) { }

  internal void RegisterConstantToken(string tokenName, string tokenValue) {
    Log.Verbose($"Registering token '{tokenName}' with constant value '{tokenValue}'.");
    this.RegisterToken(tokenName, () => new[] { tokenValue });
  }

  internal void RegisterStringToken(string tokenName, Func<string> getValue) {
    Log.Verbose($"Registering token '{tokenName}' with any possible string value.");
    this.RegisterToken(tokenName, () => new[] { getValue() });
  }

  internal void RegisterBoolToken(
      string tokenName,
      Func<bool> getValue,
      string? valueIfTrue = null,
      string? valueIfFalse = null,
      string? autoValueString = null) {

    if (!string.IsNullOrEmpty(autoValueString)) {
      (valueIfTrue, valueIfFalse) = (autoValueString, "No" + autoValueString);
    } else if (string.IsNullOrEmpty(valueIfTrue) || string.IsNullOrEmpty(valueIfFalse)) {
      (valueIfTrue, valueIfFalse) = (true.ToString(), false.ToString());
    }

    Log.Verbose(
        $"Registering token '{tokenName}' with values '{valueIfTrue}' or '{valueIfFalse}'.");
    this.RegisterToken(tokenName, () => new[] { getValue() ? valueIfTrue : valueIfFalse });
  }

  internal void RegisterIntToken(
      string tokenName,
      Func<int> getValue,
      int minValue = int.MinValue,
      int maxValue = int.MaxValue) {

    Log.Verbose($"Registering token '{tokenName}' with values between {minValue} and {maxValue}.");
    this.RegisterToken(
        tokenName, () => new[] { Math.Clamp(getValue(), minValue, maxValue).ToString() });
  }

  internal void RegisterEnumToken<T>(string tokenName, Func<T?> getValue) where T : Enum {
    Log.Verbose(
        $"Registering token '{tokenName}' with one of the following possible values: " +
        $"'{string.Join(", ", typeof(T).GetEnumNames())}'.");
    this.RegisterToken(
        tokenName, () => new[] { (getValue() is T value) ? value.ToString() : string.Empty });
  }

  internal void RegisterCompositeToken(string tokenName, Dictionary<string, Func<bool>> entries) {
    Log.Verbose(
        $"Registering token '{tokenName}' with zero, one, or multiple of the following " +
        $"possible values: '{entries.Keys.CommaJoin()}'.");
    this.RegisterToken(tokenName, () => {
      IEnumerable<string> values = entries.Where(entry => entry.Value()).Select(entry => entry.Key);
      return values.Any() ? values : new[] { string.Empty };
    });
  }

  private void RegisterToken(string tokenName, Func<IEnumerable<string>> getTokenValue) {
    if (tokenName.IsEmpty()) {
      Log.Error($"Cannot register a token with an empty name.");
    } else {
      this.Api.RegisterToken(this.Manifest, tokenName, getTokenValue);
    }
  }
}
