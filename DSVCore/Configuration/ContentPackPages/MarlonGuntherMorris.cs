using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class MarlonGuntherMorris : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Marlon : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Summer_1_Base";
      }
    }

    internal sealed class Gunther : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Fall_1_Base";
      }
    }

    internal sealed class Morris : BaseCharacterSection {
      public bool SeasonalOutfits { get; set; } = true;

      internal override void RegisterTokens() {
        TokenRegistry.AddBoolToken(
            "MorrisVariant", () => this.SeasonalOutfits,
            valueIfTrue: "Vanilla", valueIfFalse: "Off");
      }

      internal override string GetModImagePath(
          string imageDirectory, IDictionary<string, object?> ephemeralProperties) {
        var getValue = ephemeralProperties.TryGetValue;
        return (getValue(nameof(this.SeasonalOutfits), out object? value) && (value is true))
               ? $"Morris/{imageDirectory}/Morris_Fall_1_Base.png"
               : string.Empty;
      }
    }
  }

  public Sections.Marlon Marlon { get; set; } = new();
  public Sections.Gunther Gunther { get; set; } = new();
  public Sections.Morris Morris { get; set; } = new();
}
