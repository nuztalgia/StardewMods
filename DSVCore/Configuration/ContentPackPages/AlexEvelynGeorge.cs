using System;
using System.Collections.Generic;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AlexEvelynGeorge : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Alex : BaseCharacterSection.Bachelorex<FamilyVariant> {
      public bool Tattoos { get; set; } = true;

      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("JewishWeddingSuit") ? 6 : 5;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Alex>(
            "Tattoos", () => (this.Variant is FamilyVariant.Samoan) && this.Tattoos);
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        if ((variant == nameof(FamilyVariant.Samoan))
            && ephemeralState.IsTrueValue(nameof(this.Tattoos))) {
          yield return
              $"Alex/{imageDirectory}/Samoan/TattooOverlays/Alex_{this.GetPreviewOutfit()}.png";
        }
      }
    }

    internal sealed class Evelyn : BaseCharacterSection.Villager<FamilyVariant> {
      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }

    internal sealed class George : BaseCharacterSection.Villager<FamilyVariant> {
      public GeorgeBeard Beard { get; set; } = GeorgeBeard.Dynamic;

      public override string GetPreviewOutfit() {
        return "Fall_1_Rain";
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterEnumToken(
            "GeorgeBeard",
            () => this.Immersion.IsNotUltralight() ? this.Beard : GeorgeBeard.NoBeard);
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        if (ephemeralState.TryGetValue(nameof(this.Beard), out object? value)
            && Enum.TryParse(value?.ToString(), ignoreCase: true, out GeorgeBeard beard)) {
          string beardOverlayPathPrefix =
              this.GetModImagePath(imageDirectory, variant) + "BeardOverlays/George_Overlay_Beard_";
          if (beard == GeorgeBeard.Beard) {
            yield return beardOverlayPathPrefix + "Winter.png";
          } else if (beard == GeorgeBeard.Dynamic) {
            yield return beardOverlayPathPrefix + "Fall.png";
          }
        }
      }
    }
  }

  internal enum FamilyVariant {
    Mexican,
    Samoan,
    Vanilla,
    Off
  }

  internal enum GeorgeBeard {
    Beard,
    NoBeard,
    Dynamic
  }

  public Sections.Alex Alex { get; set; } = new();
  public Sections.Evelyn Evelyn { get; set; } = new();
  public Sections.George George { get; set; } = new();
}
