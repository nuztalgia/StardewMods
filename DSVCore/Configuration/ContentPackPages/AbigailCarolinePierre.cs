namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AbigailCarolinePierre : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Abigail : BaseBachelorexSection {
      public AbigailVariant Variant { get; set; } = AbigailVariant.VanillaStraightSize;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<AbigailVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("AbigailSpanishWeddingDress") ? 6 : 5;
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Summer_3_Base";
      }
    }

    internal sealed class Caroline : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Fall_1_Base";
      }
    }

    internal sealed class Pierre : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Winter_1_Base";
      }
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
}
