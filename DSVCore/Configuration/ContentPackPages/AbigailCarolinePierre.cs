namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AbigailCarolinePierre : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Abigail : BaseCharacterSection.Bachelorex<AbigailVariant> {
      public override AbigailVariant Variant { get; set; } = AbigailVariant.VanillaStraightSize;

      public override string GetPreviewOutfit() {
        return "Summer_3_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("SpanishWeddingDress") ? 6 : 5;
      }
    }

    internal sealed class Caroline : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Fall_1_Base";
      }
    }

    internal sealed class Pierre : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
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
