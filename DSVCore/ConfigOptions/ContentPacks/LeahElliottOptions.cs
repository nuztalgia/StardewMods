namespace Nuztalgia.StardewMods.DSVCore;

internal class LeahElliottOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum LeahVariant {
      Vanilla,
      Native,
      Butch,
      Off
    }
  }

  internal class LeahOptions : BaseOptions {
    public LocalOptions.LeahVariant Variant { get; set; } =
        LocalOptions.LeahVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool MaternitySprites { get; set; } = false;
  }

  internal class ElliottOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  public LeahOptions Leah { get; set; } = new();
  public ElliottOptions Elliott { get; set; } = new();
}
