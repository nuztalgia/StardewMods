namespace Nuztalgia.StardewMods.DSVCore;

internal class ShaneJasMarnieOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum ShaneSelfCare {
      Neat,
      Messy,
      Dynamic
    }

    internal enum MarnieSmile {
      Smile,
      NoSmile
    }
  }

  internal class ShaneOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.ShaneSelfCare SelfCare { get; set; } = LocalOptions.ShaneSelfCare.Dynamic;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class JasOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class MarnieOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public LocalOptions.MarnieSmile Smile { get; set; } = LocalOptions.MarnieSmile.Smile;
    public bool MermaidPendant { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  public ShaneOptions Shane { get; set; } = new();
  public JasOptions Jas { get; set; } = new();
  public MarnieOptions Marnie { get; set; } = new();
}
