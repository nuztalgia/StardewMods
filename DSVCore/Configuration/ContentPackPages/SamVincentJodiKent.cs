using Nuztalgia.StardewMods.Common.ContentPatcher;

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

      protected override void RegisterExtraTokens(Integration contentPatcher) {
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

      protected override void RegisterExtraTokens(Integration contentPatcher) {
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
