namespace Nuztalgia.StardewMods.DSVCore;

internal class EmilyHaleySandyOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum FamilyVariant {
      Vanilla,
      Black,
      Romani,
      Off
    }

    internal enum HaleyAccessories {
      BlackCam,
      Piercings,
      Cuffs
    }
  }

  public LocalOptions.FamilyVariant EmilyVariant { get; set; } =
      LocalOptions.FamilyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig EmilyLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool EmilyPlatonicNPC { get; set; } = true;
  public GlobalOptions.Pyjamas EmilyPyjamas { get; set; } = GlobalOptions.Pyjamas.True;
  public GlobalOptions.Tattoos EmilyTattoos { get; set; } = GlobalOptions.Tattoos.Tattoos;
  public int EmilyWeddingOutfit { get; set; } = 1;
  public bool EmilyMermaidPendant { get; set; } = true;
  public bool EmilyMaternitySprites { get; set; } = false;

  public LocalOptions.FamilyVariant HaleyVariant { get; set; } =
      LocalOptions.FamilyVariant.Vanilla;
  public GlobalOptions.StandardLightweightConfig HaleyLightweightConfig { get; set; } =
      GlobalOptions.StandardLightweightConfig.Full;
  public bool HaleyPlatonicNPC { get; set; } = true;
  public GlobalOptions.Pyjamas HaleyPyjamas { get; set; } = GlobalOptions.Pyjamas.True;
  public string HaleyAccessories { get; set; } = "";
  public int HaleyWeddingOutfit { get; set; } = 1;
  public bool HaleyMermaidPendant { get; set; } = true;
  public bool HaleyMaternitySprites { get; set; } = false;

  public GlobalOptions.StandardVariant SandyVariant { get; set; } =
      GlobalOptions.StandardVariant.Vanilla;
  public GlobalOptions.MinimalLightweightConfig SandyLightweightConfig { get; set; } =
      GlobalOptions.MinimalLightweightConfig.Full;
  public bool SandyGiftTastesChange { get; set; } = true;
  public bool SandyLookingForLove { get; set; } = false;
}
