using System.Collections.Generic;

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

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "HarveyGiftTastesChange",
            () => (this.Variant is HarveyVariant.ModdedSikh) && this.GiftTastesChange);
        contentPatcher.RegisterBoolToken(
            "HarveyCharacterMustache",
            () => (this.Variant is HarveyVariant.Vanilla) && this.SpriteMustache,
            autoValueString: "Mustache");
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        if ((variant == nameof(StandardVariant.Vanilla))
            && (imageDirectory == CharacterConfigState.SpritesDirectory)
            && ephemeralState.IsTrueValue(nameof(this.SpriteMustache))) {
          yield return "Harvey/Characters/Harvey_MustacheOverlay.png";
        }
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
