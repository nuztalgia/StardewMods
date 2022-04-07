namespace Nuztalgia.StardewMods.DSVCore;

internal class PennyPamOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum PennyVariant {
      Vanilla,
      ModdedAirynNotsnufffieLarge,
      ModdedAirynNotsnufffieSmall,
      Mixed,
      Off
    }
  }

  internal class PennyOptions : BaseOptions {
    public LocalOptions.PennyVariant Variant { get; set; } =
        LocalOptions.PennyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class PamOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool MermaidPendant { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  public PennyOptions Penny { get; set; } = new();
  public PamOptions Pam { get; set; } = new();
}
