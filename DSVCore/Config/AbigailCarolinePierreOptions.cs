namespace Nuztalgia.StardewMods.DSVCore;

internal class AbigailCarolinePierreOptions {

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

  public LocalOptions.AbigailVariant AbigailVariant { get; set; } =
      LocalOptions.AbigailVariant.VanillaStraightSize;
  public GlobalOptions.StandardLightweightConfig AbigailLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool AbigailPlatonicNPC { get; set; } = true;
  public GlobalOptions.Pyjamas AbigailPyjamas { get; set; } = GlobalOptions.Pyjamas.True;
  public int AbigailWeddingOutfit { get; set; } = 1;
  public bool AbigailMermaidPendant { get; set; } = true;
  public bool AbigailMaternitySprites { get; set; } = false;

  public GlobalOptions.StandardVariant CarolineVariant { get; set; } =
      GlobalOptions.StandardVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig CarolineLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;

  public GlobalOptions.StandardVariant PierreVariant { get; set; } =
      GlobalOptions.StandardVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig PierreLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;

  public GlobalOptions.MinimalVariant RidgesideVillageBert { get; set; } =
      GlobalOptions.MinimalVariant.Off;

  public LocalOptions.TrinnieVariant RidgesideVillageTrinnie { get; set; } =
      LocalOptions.TrinnieVariant.Off;
}
