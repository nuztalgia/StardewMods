namespace Nuztalgia.StardewMods.DSVCore;

internal class SebastianMaruRobinDemetriusOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum SebastianVariant {
      Vanilla,
      ModdedBlack,
      ModdedPurple,
      Off
    }

    internal enum MaruVariant {
      Vanilla,
      ModdedNotsnufffie,
      ModdedLavender,
      Off
    }

    internal enum DemetriusVariant {
      Vanilla,
      ModdedDarkSkin,
      ModdedAlbino,
      Off
    }

    internal enum MaruScrubs {
      Scrubs,
      NoScrubs
    }

    internal enum MaruSpriteGlasses {
      Glasses,
      NoGlasses
    }

    internal enum SebastianSafety {
      Helmet,
      NoHelmet
    }

    internal enum SebastianGlasses {
      Dynamic,
      Glasses,
      NoGlasses
    }

    internal enum SebastianPiercings {
      Piercings,
      NoPiercings
    }
  }

  internal class SebastianOptions : BaseOptions {
    public LocalOptions.SebastianVariant Variant { get; set; } =
        LocalOptions.SebastianVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.SebastianSafety Safety { get; set; } =
        LocalOptions.SebastianSafety.Helmet;
    public LocalOptions.SebastianGlasses Glasses { get; set; } =
        LocalOptions.SebastianGlasses.NoGlasses;
    public LocalOptions.SebastianPiercings Piercings { get; set; } =
        LocalOptions.SebastianPiercings.NoPiercings;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class MaruOptions : BaseOptions {
    public LocalOptions.MaruVariant Variant { get; set; } =
        LocalOptions.MaruVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.MaruScrubs Scrubs { get; set; } = LocalOptions.MaruScrubs.Scrubs;
    public LocalOptions.MaruSpriteGlasses SpriteGlasses { get; set; } =
        LocalOptions.MaruSpriteGlasses.NoGlasses;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool MaternitySprites { get; set; } = false;
  }

  internal class RobinOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class DemetriusOptions : BaseOptions {
    public LocalOptions.DemetriusVariant Variant { get; set; } =
        LocalOptions.DemetriusVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  public SebastianOptions Sebastian { get; set; } = new();
  public MaruOptions Maru { get; set; } = new();
  public RobinOptions Robin { get; set; } = new();
  public DemetriusOptions Demetrius { get; set; } = new();
}
