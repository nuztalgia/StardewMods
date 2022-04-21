namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class PennyPam : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Penny : BaseCharacterSection.Bachelorex<PennyVariant> {
      public override string GetPreviewOutfit() {
        return "Spring_1_Sun";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("IrishWeddingDress") ? 6 : 5;
      }
    }

    internal sealed class Pam : BaseCharacterSection.Villager<StandardVariant>,
        IHasCustomModImageDirectory {
      public override string GetPreviewOutfit() {
        return "Summer_1_Sun";
      }
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
