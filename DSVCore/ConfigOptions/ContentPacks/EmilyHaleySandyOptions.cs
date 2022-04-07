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

  internal class EmilyOptions : BaseOptions {
    public LocalOptions.FamilyVariant Variant { get; set; } =
        LocalOptions.FamilyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public GlobalOptions.Tattoos Tattoos { get; set; } = GlobalOptions.Tattoos.Tattoos;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool MaternitySprites { get; set; } = false;
  }

  internal class HaleyOptions : BaseOptions {
    public LocalOptions.FamilyVariant Variant { get; set; } =
        LocalOptions.FamilyVariant.Vanilla;
    public GlobalOptions.StandardLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.StandardLightweightConfig.Full;
    public bool PlatonicNpc { get; set; } = true;
    public GlobalOptions.Pyjamas Pyjamas { get; set; } = GlobalOptions.Pyjamas.True;
    public LocalOptions.HaleyAccessories Accessories { get; set; } =
        LocalOptions.HaleyAccessories.BlackCam;
    public int WeddingOutfit { get; set; } = 1;
    public bool MermaidPendant { get; set; } = true;
    public bool MaternitySprites { get; set; } = false;
  }

  internal class SandyOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.MinimalLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.MinimalLightweightConfig.Full;
    public bool GiftTastesChange { get; set; } = true;
    public bool LookingForLove { get; set; } = false;
  }

  public EmilyOptions Emily { get; set; } = new();
  public HaleyOptions Haley { get; set; } = new();
  public SandyOptions Sandy { get; set; } = new();
}
