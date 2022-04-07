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

  internal class HarveyOptions : BaseOptions {
    public LocalOptions.HarveyVariant Variant { get; set; } =
        LocalOptions.HarveyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = false;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.HarveyCharacterMustache CharacterMustache { get; set; } =
        LocalOptions.HarveyCharacterMustache.NoMustache;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool GiftTastesChange { get; set; } = true;
  }

  internal class GusOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool LookingForLove { get; set; } = false;
  }

  internal class LewisOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool LookingForLove { get; set; } = false;
  }

  internal class RidgesideVillageOptions : BaseOptions {
    public GlobalOptions.MinimalVariant Lenny { get; set; } =
        GlobalOptions.MinimalVariant.Off;
  }

  public HarveyOptions Harvey { get; set; } = new();
  public GusOptions Gus { get; set; } = new();
  public LewisOptions Lewis { get; set; } = new();
  public RidgesideVillageOptions RidgesideVillage { get; set; } = new();
}
