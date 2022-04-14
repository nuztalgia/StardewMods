namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class SebastianMaruRobinDemetrius : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Sebastian : BaseBachelorexSection {
      public SebastianVariant Variant { get; set; } = SebastianVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Helmet { get; set; } = true;
      public bool Piercings { get; set; } = false;
      public SebastianGlasses Glasses { get; set; } = SebastianGlasses.NoGlasses;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SebastianVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        this.RegisterAutoNamedBoolToken("Helmet", () => this.Helmet);
        this.RegisterAutoNamedBoolToken("Piercings",
            () => this.Immersion.IsNotUltralight() && this.Piercings);
        TokenRegistry.AddEnumToken<SebastianGlasses>("SebastianGlasses",
            () => this.Immersion.IsNotUltralight() ? this.Glasses : SebastianGlasses.NoGlasses);
      }

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

      internal override void RegisterTokens() {
        this.RegisterVariantToken<MaruVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        this.RegisterAutoNamedBoolToken("Scrubs", () => this.Scrubs);
        TokenRegistry.AddBoolToken(
            "MaruCharacterGlasses",
            () => (this.Variant is MaruVariant.Vanilla or MaruVariant.ModdedNotsnufffie)
                  && this.SpriteGlasses,
            autoValueString: "Glasses");
      }

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

      internal override void RegisterTokens() {
        this.RegisterVariantToken<DemetriusVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
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
