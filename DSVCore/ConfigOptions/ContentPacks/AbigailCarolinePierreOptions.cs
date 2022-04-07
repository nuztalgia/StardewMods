namespace Nuztalgia.StardewMods.DSVCore;

internal class AbigailCarolinePierreOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum AbigailVariant {
      VanillaStraightSize,
      VanillaPlusSize,
      ModdedStraightSize,
      ModdedPlusSize,
      Off
    }

    internal enum TrinnieVariant {
      ModdedBlack,
      ModdedGreen,
      Off
    }
  }

  internal class AbigailOptions : BaseOptions {
    public LocalOptions.AbigailVariant Variant { get; set; } =
        LocalOptions.AbigailVariant.VanillaStraightSize;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool MaternitySprites { get; set; } = false;
  }

  internal class CarolineOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class PierreOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class RidgesideVillageOptions : BaseOptions {
    public GlobalOptions.MinimalVariant Bert { get; set; } =
        GlobalOptions.MinimalVariant.Off;
    public LocalOptions.TrinnieVariant Trinnie { get; set; } =
        LocalOptions.TrinnieVariant.Off;
  }

  public AbigailOptions Abigail { get; set; } = new();
  public CarolineOptions Caroline { get; set; } = new();
  public PierreOptions Pierre { get; set; } = new();
  public RidgesideVillageOptions RidgesideVillage { get; set; } = new();
}
