namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class PennyPam : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Penny : BaseBachelorexSection {
      public PennyVariant Variant { get; set; } = PennyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

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
    MixedRace,
    ModdedWhiteSmaller,
    ModdedWhiteLarger,
    Vanilla,
    Off
  }

  public Sections.Penny Penny { get; set; } = new();
  public Sections.Pam Pam { get; set; } = new();
}
