namespace Nuztalgia.StardewMods.DSVCore;

internal class SamVincentJodiKentOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum SamVariant {
      Vanilla,
      ModdedLighter,
      ModdedDarker,
      Off
    }

    internal enum SamEyes {
      Default,
      Alternate,
      Heterochromia
    }

    internal enum SamExtras {
      Stubble,
      Beard,
      Piercings
    }

    internal enum SamBinder {
      Binder,
      NoBinder
    }
  }

  internal class SamOptions : BaseOptions {
    public LocalOptions.SamVariant Variant { get; set; } =
        LocalOptions.SamVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.SamEyes Eyes { get; set; } = LocalOptions.SamEyes.Default;
    public LocalOptions.SamExtras Extras { get; set; } = LocalOptions.SamExtras.Stubble;
    public LocalOptions.SamBinder Binder { get; set; } = LocalOptions.SamBinder.Binder;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class VincentOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class JodiOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool GiftTastesChange { get; set; } = true;
  }

  internal class KentOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  public SamOptions Sam { get; set; } = new();
  public VincentOptions Vincent { get; set; } = new();
  public JodiOptions Jodi { get; set; } = new();
  public KentOptions Kent { get; set; } = new();
}
