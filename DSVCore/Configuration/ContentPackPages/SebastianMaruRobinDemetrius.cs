using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SebastianMaruRobinDemetrius : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sebastian : BaseCharacterSection.Bachelorex<SebastianVariant> {
      public bool Helmet { get; set; } = true;
      public bool Piercings { get; set; } = false;
      public SebastianGlasses Glasses { get; set; } = SebastianGlasses.NoGlasses;

      public override string GetPreviewOutfit() {
        return "Winter_1_Sun";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("DutchRomanianOrDutchVietnameseWeddingOutfits") ? 6 : 4;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Sebastian>("Helmet", () => this.Helmet);
        contentPatcher.RegisterAutoNamedBoolToken<Sebastian>(
            "Piercings", () => this.Immersion.IsNotUltralight() && this.Piercings);
        contentPatcher.RegisterEnumToken(
            "SebastianGlasses",
            () => this.Immersion.IsNotUltralight() ? this.Glasses : SebastianGlasses.NoGlasses);
      }
    }

    internal sealed class Maru : BaseCharacterSection.Bachelorex<MaruVariant> {
      public bool Scrubs { get; set; } = true;
      public bool SpriteGlasses { get; set; } = false;

      public override string GetPreviewOutfit() {
        return "Summer_2_Rain";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("MaruDutchXhosaWeddingDress") ? 6 : 5;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Maru>("Scrubs", () => this.Scrubs);
        contentPatcher.RegisterBoolToken(
            "MaruCharacterGlasses",
            () => (this.Variant is MaruVariant.Vanilla or MaruVariant.ModdedNotsnufffie)
                && this.SpriteGlasses,
            autoValueString: "Glasses");
      }
    }

    internal sealed class Robin : BaseCharacterSection.Villager<StandardVariant> {
      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }

    internal sealed class Demetrius : BaseCharacterSection.Villager<DemetriusVariant> {
      public override string GetPreviewOutfit() {
        return "Spring_1_Base";
      }
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
