namespace Nuztalgia.StardewMods.DSVCore;

internal class AlexEvelynGeorgeOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum FamilyVariant {
      Vanilla,
      Samoan,
      Mexican,
      Off
    }

    internal enum GeorgeBeard {
      Beard,
      NoBeard,
      Dynamic
    }
  }

  internal class AlexOptions : BaseOptions {
    public LocalOptions.FamilyVariant Variant { get; set; } =
        LocalOptions.FamilyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = false;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public GlobalOptions.Tattoos Tattoos { get; set; } = GlobalOptions.Tattoos.Tattoos;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class EvelynOptions : BaseOptions {
    public LocalOptions.FamilyVariant Variant { get; set; } =
        LocalOptions.FamilyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
  }

  internal class GeorgeOptions : BaseOptions {
    public LocalOptions.FamilyVariant Variant { get; set; } =
        LocalOptions.FamilyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public LocalOptions.GeorgeBeard Beard { get; set; } = LocalOptions.GeorgeBeard.Dynamic;
  }

  public AlexOptions Alex { get; set; } = new();
  public EvelynOptions Evelyn { get; set; } = new();
  public GeorgeOptions George { get; set; } = new();
}
