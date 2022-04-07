namespace Nuztalgia.StardewMods.DSVCore;

internal class WillyClintLinusOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum WillyVariant {
      Vanilla,
      Disabled,
      Tongan,
      TonganDisabled,
      Off
    }

    internal enum LinusVariant {
      Vanilla,
      Off
    }

    internal enum ClintScar {
      Scar,
      NoScar
    }
  }

  internal class WillyOptions : BaseOptions {
    public LocalOptions.WillyVariant Variant { get; set; } =
        LocalOptions.WillyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool MermaidPendant { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  internal class ClintOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public LocalOptions.ClintScar Scar { get; set; } = LocalOptions.ClintScar.NoScar;
    public bool MermaidPendant { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  internal class LinusOptions : BaseOptions {
    public LocalOptions.LinusVariant Variant { get; set; } =
        LocalOptions.LinusVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool MermaidPendant { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  public WillyOptions Willy { get; set; } = new();
  public ClintOptions Clint { get; set; } = new();
  public LinusOptions Linus { get; set; } = new();
}
