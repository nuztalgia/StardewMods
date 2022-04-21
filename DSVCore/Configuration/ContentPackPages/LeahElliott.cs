namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class LeahElliott : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Leah : BaseCharacterSection.Bachelorex<LeahVariant> {
      public override string GetPreviewOutfit() {
        return "Fall_2_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("PolishWeddingOutfits") ? 7 : 5;
      }
    }

    internal sealed class Elliott : BaseCharacterSection.Bachelorex<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Fall_1_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("ScottishWeddingKilt") ? 6 : 5;
      }
    }
  }

  internal enum LeahVariant {
    Native,
    Butch,
    Vanilla,
    Off
  }

  public Sections.Leah Leah { get; set; } = new();
  public Sections.Elliott Elliott { get; set; } = new();
}
