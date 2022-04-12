using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class KrobusMermaidsWizardWitch : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Krobus : BaseCharacterSection {
      public SimpleVariant Variant { get; set; } = SimpleVariant.Modded;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
    }

    internal sealed class Mermaids : BaseCharacterSection {
      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        this.AddTokenByProperty(tokenMap, nameof(this.Randomization), customSuffix: "");
      }
    }

    internal sealed class Wizard : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool HatJunimos { get; set; } = false;
      public bool ShoulderJunimos { get; set; } = false;
      public bool SpiritCreatures { get; set; } = false;
      public WizardMarriageMod MarriageMod { get; set; } = WizardMarriageMod.None;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(tokenMap, nameof(this.MarriageMod));
        tokenMap.Add("WizardFamiliars", () => this.GetCombinedTokenValues(
            nameof(this.HatJunimos), nameof(this.ShoulderJunimos), nameof(this.SpiritCreatures)));
      }
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
    Light,
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
