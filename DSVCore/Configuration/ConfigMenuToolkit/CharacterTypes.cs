using System;

namespace Nuztalgia.StardewMods.DSVCore;

internal enum StandardVariant {
  Modded,
  Vanilla,
  Off
}

internal enum StandardImmersion {
  Full,
  Light,
  Ultralight
}

internal enum SimpleImmersion {
  Full,
  Light
}

internal interface IHasCustomDisplayName {
  string GetDisplayName();
}

internal interface IHasCustomPreviewTooltip {
  string GetPreviewTooltip() {
    return I18n.Tooltip_Preview_Singular();
  }
}

internal interface IHasCustomModImagePath {
  string GetModImagePath(string imageDirectory);
}

internal interface IHasCustomModImageDirectory {
  string GetDirectory(string variant) {
    return $"Default/{variant}";
  }
}

internal interface IHasVariant {
  void RegisterVariantToken(string name, ContentPatcherIntegration contentPatcher);

  string GetPreviewOutfit() {
    throw new NotImplementedException("Character did not specify a preview outfit.");
  }
}

internal interface IHasVariant<TVariant> : IHasVariant where TVariant : Enum {
  abstract TVariant Variant { get; set; }

  void IHasVariant.RegisterVariantToken(string characterName, ContentPatcherIntegration cp) {
    if (this.Variant.GetType().Name.EndsWith(nameof(this.Variant))) {
      cp.RegisterEnumToken(characterName + "Variant", () => this.Variant);
    } else {
      Log.Error(
          $"Skipping registration of '{characterName}Variant' custom token. " +
          $"('{this.Variant.GetType().Name}' doesn't sound like a variant type.)");
    }
  }
}

internal interface IHasImmersion {
  void RegisterImmersionToken(string name, ContentPatcherIntegration contentPatcher);
}

internal interface IHasImmersion<TImmersion> : IHasImmersion where TImmersion : Enum {
  abstract TImmersion Immersion { get; set; }

  void IHasImmersion.RegisterImmersionToken(string characterName, ContentPatcherIntegration cp) {
    if (this.Immersion is StandardImmersion or SimpleImmersion) {
      cp.RegisterEnumToken(characterName + "LightweightConfig", () => this.Immersion);
    } else {
      Log.Error(
          $"Skipping registration of '{characterName}LightweightConfig' custom token. " +
          $"('{this.Immersion.GetType().Name}' is not a recognized immersion type.)");
    }
  }
}

internal interface IHasWeddingOutfit {
  const int FirstWeddingOutfit = 1;

  public int WeddingOutfit { get; set; }

  void RegisterWeddingOutfitToken(string characterName, ContentPatcherIntegration cp) {
    cp.RegisterIntToken(
        characterName + "WeddingOutfit",
        () => this.WeddingOutfit,
        FirstWeddingOutfit,
        this.GetNumberOfWeddingOutfits());
  }

  int GetNumberOfWeddingOutfits();
}

internal static class CharacterTypeExtensions {

  internal static bool IsModded(this StandardVariant variant) {
    return variant != StandardVariant.Modded;
  }

  internal static bool IsNotUltralight(this StandardImmersion immersion) {
    return immersion != StandardImmersion.Ultralight;
  }

  internal static bool IsFull(this StandardImmersion immersion) {
    return immersion == StandardImmersion.Full;
  }

  internal static bool IsFull(this SimpleImmersion immersion) {
    return immersion == SimpleImmersion.Full;
  }

  // Extension method for ContentPatcher Integration to handle character bools with a naming scheme.
  internal static void RegisterAutoNamedBoolToken<TCharacter>(
      this ContentPatcherIntegration cp, string tokenName, Func<bool> getValue)
          where TCharacter : BaseCharacterSection {
    cp.RegisterBoolToken(typeof(TCharacter).Name + tokenName, getValue, autoValueString: tokenName);
  }
}
