namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SebastianMaruRobinDemetrius : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sebastian : BaseBachelorexSection {
      public SebastianVariant Variant { get; set; } = SebastianVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public SebastianGlasses Glasses { get; set; } = SebastianGlasses.NoGlasses;
      public bool Helmet { get; set; } = true;
      public bool Piercings { get; set; } = false;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("SebastianDutchRomanianOrDutchVietnameseWeddingOutfits") ? 6 : 4;
      }
    }

    internal sealed class Maru : BaseBachelorexSection {
      public MaruVariant Variant { get; set; } = MaruVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Scrubs { get; set; } = true;
      public bool SpriteGlasses { get; set; } = false;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("MaruDutchXhosaWeddingDress") ? 6 : 5;
      }
    }

    internal sealed class Robin : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Demetrius : BaseCharacterSection {
      public DemetriusVariant Variant { get; set; } = DemetriusVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }
  }

  internal enum SebastianVariant {
    ModdedBlack,
    ModdedPurple,
    Vanilla,
    Off
  }

  internal enum SebastianGlasses {
    Glasses,
    NoGlasses,
    Dynamic
  }

  internal enum MaruVariant {
    ModdedNotsnufffie,
    ModdedLavender,
    Vanilla,
    Off
  }

  internal enum DemetriusVariant {
    ModdedDarkSkin,
    ModdedAlbino,
    Vanilla,
    Off
  }

  public Sections.Sebastian Sebastian { get; set; } = new();
  public Sections.Maru Maru { get; set; } = new();
  public Sections.Robin Robin { get; set; } = new();
  public Sections.Demetrius Demetrius { get; set; } = new();
}
