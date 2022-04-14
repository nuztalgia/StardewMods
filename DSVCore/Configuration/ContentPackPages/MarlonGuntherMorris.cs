namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class MarlonGuntherMorris : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Marlon : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
      }
    }

    internal sealed class Gunther : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
      }
    }

    internal sealed class Morris : BaseCharacterSection {
      public bool SeasonalOutfits { get; set; } = true;

      internal override void RegisterTokens() {
        TokenRegistry.AddBoolToken(
            "MorrisVariant", () => this.SeasonalOutfits,
            valueIfTrue: "Vanilla", valueIfFalse: "Off");
      }
    }
  }

  public Sections.Marlon Marlon { get; set; } = new();
  public Sections.Gunther Gunther { get; set; } = new();
  public Sections.Morris Morris { get; set; } = new();
}
