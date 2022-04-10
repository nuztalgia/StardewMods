namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AbigailCarolinePierre : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Abigail : BaseBachelorexSection {
      public AbigailVariant Variant { get; set; } = AbigailVariant.VanillaStraightSize;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("AbigailSpanishWeddingDress") ? 6 : 5;
      }
    }

    internal sealed class Caroline : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Pierre : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }
  }

  internal enum AbigailVariant {
    ModdedPlusSize,
    ModdedStraightSize,
    VanillaPlusSize,
    VanillaStraightSize,
    Off
  }

  public Sections.Abigail Abigail { get; set; } = new();
  public Sections.Caroline Caroline { get; set; } = new();
  public Sections.Pierre Pierre { get; set; } = new();
  public CompatSections.RidgesideVillage.Bert Bert { get; set; } = new();
  public CompatSections.RidgesideVillage.Trinnie Trinnie { get; set; } = new();
}
