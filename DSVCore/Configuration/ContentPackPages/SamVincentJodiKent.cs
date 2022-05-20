namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SamVincentJodiKent : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sam : BaseCharacterSection.Bachelorex<SamVariant> {
      public bool Binder { get; set; } = true;
      public SamEyeColor EyeColor { get; set; } = SamEyeColor.Default;
      public bool Beard { get; set; } = false;
      public bool Stubble { get; set; } = false;
      public bool Piercings { get; set; } = false;

      public override string GetPreviewOutfit() {
        return "Fall_1_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("NorwegianWeddingSuit") ? 4 : 3;
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Sam>("Binder", () => this.Binder);
        contentPatcher.RegisterEnumToken(
            "SamEyes",
            () => this.Immersion.IsNotUltralight() ? this.EyeColor : SamEyeColor.Default);
        contentPatcher.RegisterCompositeToken("SamExtras", new() {
          ["Beard"] = () => this.Immersion.IsNotUltralight() && this.Beard,
          ["Stubble"] = () => this.Immersion.IsNotUltralight() && this.Stubble,
          ["Piercings"] = () => this.Immersion.IsNotUltralight() && this.Piercings
        });
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        string overlayPathPrefix = this.GetModImagePath(imageDirectory, variant) + "Overlays/Sam_";

        if (ephemeralState.TryGetValue(nameof(this.EyeColor), out object? value)
            && Enum.TryParse(value?.ToString(), ignoreCase: true, out SamEyeColor eyeColor)
            && (eyeColor is SamEyeColor.Alternate or SamEyeColor.Heterochromia)) {
          yield return $"{overlayPathPrefix}{eyeColor}_EyesOverlay.png";
        }

        if ((imageDirectory == CharacterConfigState.PortraitsDirectory)
            && ephemeralState.IsTrueValue(nameof(this.Piercings))) {
          yield return $"Sam/Portraits/Sam_PiercingsOverlay.png";
        }

        if (ephemeralState.IsTrueValue(nameof(this.Stubble))) {
          yield return overlayPathPrefix + "StubbleOverlay.png";
        }

        if (ephemeralState.IsTrueValue(nameof(this.Beard))) {
          yield return overlayPathPrefix + "BeardOverlay.png";
        }
      }
    }

    internal sealed class Vincent : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }

    internal sealed class Jodi : BaseCharacterSection.Villager<StandardVariant> {
      public bool GiftTastesChange { get; set; } = true;

      public override string GetPreviewOutfit() {
        return "Summer_2_Rain";
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "JodiGiftTastesChange", () => this.Variant.IsModded() && this.GiftTastesChange);
      }
    }

    internal sealed class Kent : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Fall_1_Rain";
      }
    }
  }
  
  internal enum SamVariant {
    ModdedDarker,
    ModdedLighter,
    Vanilla,
    Off
  }

  internal enum SamEyeColor {
    Default,
    Alternate,
    Heterochromia
  }

  public Sections.Sam Sam { get; set; } = new();
  public Sections.Vincent Vincent { get; set; } = new();
  public Sections.Jodi Jodi { get; set; } = new();
  public Sections.Kent Kent { get; set; } = new();
}
