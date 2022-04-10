namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class KrobusMermaidsWizardWitch : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Krobus : BaseCharacterSection {
      public SimpleVariant Variant { get; set; } = SimpleVariant.Modded;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
    }

    internal sealed class Mermaids : BaseCharacterSection {
      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;
    }

    internal sealed class Wizard : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool HatJunimos { get; set; } = false;
      public bool ShoulderJunimos { get; set; } = false;
      public bool SpiritCreatures { get; set; } = false;
      public WizardMarriageMod MarriageMod { get; set; } = WizardMarriageMod.None;
    }

    internal sealed class Witch : BaseCharacterSection {
      public SimpleVariant Variant { get; set; } = SimpleVariant.Off;
    }
  }

  internal enum SimpleVariant {
    Modded,
    Off
  }

  internal enum MermaidRandomization {
    RandomBoth,
    RandomUpper,
    RandomLower,
    Lightweight,
    Off
  }

  internal enum WizardMarriageMod {
    LookingForLove,
    RomanceableRasmodius,
    None
  }

  public Sections.Krobus Krobus { get; set; } = new();
  public Sections.Mermaids Mermaids { get; set; } = new();
  public Sections.Wizard Wizard { get; set; } = new();
  public Sections.Witch Witch { get; set; } = new();
}
