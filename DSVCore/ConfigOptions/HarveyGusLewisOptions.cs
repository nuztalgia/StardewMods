namespace Nuztalgia.StardewMods.DSVCore;

internal class HarveyGusLewisOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum HarveyVariant {
      Vanilla,
      ModdedSikh,
      ModdedNonSikh,
      Off
    }

    internal enum HarveyCharacterMustache {
      Mustache,
      NoMustache
    }
  }

  public LocalOptions.HarveyVariant HarveyVariant { get; set; } =
      LocalOptions.HarveyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig HarveyLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool HarveyPlatonicNPC { get; set; } = false;
  public GlobalOptions.Pyjamas HarveyPyjamas { get; set; } = GlobalOptions.Pyjamas.True;
  public LocalOptions.HarveyCharacterMustache HarveyCharacterMustache { get; set; } =
      LocalOptions.HarveyCharacterMustache.NoMustache;
  public int HarveyWeddingOutfit { get; set; } = 1;
  public bool HarveyMermaidPendant { get; set; } = true;
  public bool HarveyGiftTastesChange { get; set; } = true;

  public GlobalOptions.StandardVariant GusVariant { get; set; } =
      GlobalOptions.StandardVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig GusLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool GusLookingForLove { get; set; } = false;

  public GlobalOptions.StandardVariant LewisVariant { get; set; } =
      GlobalOptions.StandardVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig LewisLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool LewisLookingForLove { get; set; } = false;

  public GlobalOptions.MinimalVariant RidgesideVillageLenny { get; set; } =
      GlobalOptions.MinimalVariant.Off;
}
