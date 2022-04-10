namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SamVincentJodiKent : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sam : BaseBachelorexSection {
      public SamVariant Variant { get; set; } = SamVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
      public int WeddingOutfit { get; set; } = 1;
      public SamEyeColor EyeColor { get; set; } = SamEyeColor.Default;
      public bool Beard { get; set; } = false;
      public bool Stubble { get; set; } = false;
      public bool Piercings { get; set; } = false;
      public bool Binder { get; set; } = true;

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
