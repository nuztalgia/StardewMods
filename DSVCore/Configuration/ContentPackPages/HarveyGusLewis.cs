using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class HarveyGusLewis : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Harvey : BaseCharacterSection.Bachelorex<HarveyVariant> {
      public bool GiftTastesChange { get; set; } = true;
      public bool SpriteMustache { get; set; } = false;

      public override string GetPreviewOutfit() {
        return "Fall_1_Sun";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("HungarianWeddingSuit") ? 5 : 4;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "HarveyGiftTastesChange",
            () => (this.Variant is HarveyVariant.ModdedSikh) && this.GiftTastesChange);
        contentPatcher.RegisterBoolToken(
            "HarveyCharacterMustache",
            () => (this.Variant is HarveyVariant.Vanilla) && this.SpriteMustache,
            autoValueString: "Mustache");
      }
    }

    internal sealed class Gus : BaseCharacterSection.Villager<StandardVariant>,
        IHasCustomModImageDirectory {
      public override string GetPreviewOutfit() {
        return "Spring_1_Rain";
      }
    }

    internal sealed class Lewis : BaseCharacterSection.Villager<StandardVariant>,
        IHasCustomModImageDirectory {
      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }
  }

  internal enum HarveyVariant {
    ModdedSikh,
    ModdedNonSikh,
    Vanilla,
    Off
  }

  public Sections.Harvey Harvey { get; set; } = new();
  public Sections.Gus Gus { get; set; } = new();
  public Sections.Lewis Lewis { get; set; } = new();
}
