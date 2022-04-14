namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class PennyPam : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Penny : BaseBachelorexSection {
      public PennyVariant Variant { get; set; } = PennyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<PennyVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("PennyIrishWeddingDress") ? 6 : 5;
      }
    }

    internal sealed class Pam : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }
  }

  internal enum PennyVariant {
    Mixed,
    ModdedAirynNotsnufffieSmall,
    ModdedAirynNotsnufffieLarge,
    Vanilla,
    Off
  }

  public Sections.Penny Penny { get; set; } = new();
  public Sections.Pam Pam { get; set; } = new();
}
