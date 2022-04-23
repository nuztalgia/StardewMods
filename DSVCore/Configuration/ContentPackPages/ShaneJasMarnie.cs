using System.Collections.Generic;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class ShaneJasMarnie : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Shane : BaseCharacterSection.Bachelorex<StandardVariant>,
        IHasCustomModImageDirectory {
      public ShaneSelfCare SelfCare { get; set; } = ShaneSelfCare.Dynamic;

      public override string GetPreviewOutfit() {
        return $"Fall_{((this.SelfCare == ShaneSelfCare.Messy) ? 1 : 2)}_Sun";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("ShaneGeorgianWeddingSuit") ? 6 : 5;
      }

      string IHasCustomModImageDirectory.GetDirectory(string variant) {
        return $"{variant}/{((this.SelfCare == ShaneSelfCare.Messy) ? "Messy" : "Neat")}";
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        // TODO: Determine what should happen if Immersion is Ultralight and SelfCare is Dynamic.
        contentPatcher.RegisterEnumToken("ShaneSelfCare", () => this.SelfCare);
      }
    }

    internal sealed class Jas : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Fall_1_Base";
      }
    }

    internal sealed class Marnie : BaseCharacterSection.Villager<StandardVariant>,
        IHasCustomModImageDirectory {
      public bool SpriteSmile { get; set; } = true;

      public override string GetPreviewOutfit() {
        return "Spring_1_Sun";
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "MarnieCharacterSmile",
            () => (this.Variant is StandardVariant.Vanilla) && this.SpriteSmile,
            autoValueString: "Smile");
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralProperties) {
        if ((variant == nameof(StandardVariant.Vanilla))
            && (imageDirectory == ImagePreviewOptions.SpritesDirectory)
            && ephemeralProperties.IsFalseValue(nameof(this.SpriteSmile))) {
          yield return "Marnie/Characters/Marnie_NoSmileOverlay_Default.png";
        }
      }
    }
  }

  internal enum ShaneSelfCare {
    Neat,
    Messy,
    Dynamic
  }

  public Sections.Shane Shane { get; set; } = new();
  public Sections.Jas Jas { get; set; } = new();
  public Sections.Marnie Marnie { get; set; } = new();
}
