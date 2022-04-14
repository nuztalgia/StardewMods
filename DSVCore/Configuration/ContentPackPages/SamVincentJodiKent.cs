namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SamVincentJodiKent : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sam : BaseBachelorexSection {
      public SamVariant Variant { get; set; } = SamVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Binder { get; set; } = true;
      public SamEyeColor EyeColor { get; set; } = SamEyeColor.Default;
      public bool Beard { get; set; } = false;
      public bool Stubble { get; set; } = false;
      public bool Piercings { get; set; } = false;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SamVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        this.RegisterAutoNamedBoolToken("Binder", () => this.Binder);
        TokenRegistry.AddEnumToken<SamEyeColor>("SamEyes",
            () => this.Immersion.IsNotUltralight() ? this.EyeColor : SamEyeColor.Default);
        TokenRegistry.AddCompositeToken("SamExtras", new() {
          ["Beard"] = () => this.Immersion.IsNotUltralight() && this.Beard,
          ["Stubble"] = () => this.Immersion.IsNotUltralight() && this.Stubble,
          ["Piercings"] = () => this.Immersion.IsNotUltralight() && this.Piercings
        });
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("SamNorwegianWeddingSuit") ? 4 : 3;
      }
    }

    internal sealed class Vincent : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Jodi : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public bool GiftTastesChange { get; set; } = true;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
        TokenRegistry.AddBoolToken(
            "JodiGiftTastesChange", () => this.Variant.IsModded() && this.GiftTastesChange);
      }
    }

    internal sealed class Kent : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
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
