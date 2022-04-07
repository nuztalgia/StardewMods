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

  public LocalOptions.FamilyVariant AlexVariant { get; set; } =
      LocalOptions.FamilyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig AlexLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool AlexPlatonicNPC { get; set; } = false;
  public GlobalOptions.Pyjamas AlexPyjamas { get; set; } = GlobalOptions.Pyjamas.True;
  public GlobalOptions.Tattoos AlexTattoos { get; set; } = GlobalOptions.Tattoos.Tattoos;
  public int AlexWeddingOutfit { get; set; } = 1;
  public bool AlexMermaidPendant { get; set; } = true;

  public LocalOptions.FamilyVariant EvelynVariant { get; set; } =
      LocalOptions.FamilyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig EvelynLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;

  public LocalOptions.FamilyVariant GeorgeVariant { get; set; } =
      LocalOptions.FamilyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig GeorgeLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public LocalOptions.GeorgeBeard GeorgeBeard { get; set; } = LocalOptions.GeorgeBeard.Dynamic;
}
