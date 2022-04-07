namespace Nuztalgia.StardewMods.DSVCore;

internal class KrobusMermaidsWizardWitchOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum MermaidRandomization {
      RandomBoth,
      RandomUpper,
      RandomLower,
      Light,
      Off
    }

    internal enum WizardFamiliars {
      HatJunimos,
      ShoulderJunimos,
      SpiritCreatures
    }

    internal enum WizardMarriageMod {
      LookingForLove,
      RomanceableRasmodius,
      None
    }
  }

  internal class KrobusOptions : BaseOptions {
    public GlobalOptions.MinimalVariant Variant { get; set; } =
        GlobalOptions.MinimalVariant.Off;
    public GlobalOptions.MinimalLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.MinimalLightweightConfig.Full;
  }

  internal class MermaidsOptions : BaseOptions {
    public LocalOptions.MermaidRandomization Randomization { get; set; } =
        LocalOptions.MermaidRandomization.RandomBoth;
  }

  internal class WizardOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public GlobalOptions.MinimalLightweightConfig LightweightConfig { get; set; } =
        GlobalOptions.MinimalLightweightConfig.Full;
    public LocalOptions.WizardFamiliars Familiars { get; set; } =
        LocalOptions.WizardFamiliars.HatJunimos;
    public LocalOptions.WizardMarriageMod MarriageMod { get; set; } =
        LocalOptions.WizardMarriageMod.None;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class WitchOptions : BaseOptions {
    public GlobalOptions.MinimalVariant Variant { get; set; } =
        GlobalOptions.MinimalVariant.Off;
  }

  public KrobusOptions Krobus { get; set; } = new();
  public MermaidsOptions Mermaids { get; set; } = new();
  public WizardOptions Wizard { get; set; } = new();
  public WitchOptions Witch { get; set; } = new();
}
