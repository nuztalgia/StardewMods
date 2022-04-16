using Microsoft.Xna.Framework;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class KrobusMermaidsWizardWitch : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Krobus : BaseCharacterSection {
      public SimpleVariant Variant { get; set; } = SimpleVariant.Modded;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SimpleVariant>(() => this.Variant);
        this.RegisterImmersionToken<SimpleImmersion>(() => this.Immersion);
      }

      internal override Rectangle? GetSpriteRect() {
        return new Rectangle(0, 0, 16, 24); // He's a smol boi.
      }

      protected override string GetPreviewImagePath(string imageDirectory, string _) {
        return $"Krobus/{imageDirectory}/Krobus_1_Snow";
      }
    }

    internal sealed class Mermaids : BaseCharacterSection {
      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;

      internal override void RegisterTokens() {
        TokenRegistry.AddEnumToken<MermaidRandomization>("Mermaids", () => this.Randomization);
      }

      internal override Rectangle? GetPortraitRect() {
        return null; // No portrait available for Mermaids.
      }

      internal override Rectangle? GetSpriteRect() {
        return null; // TODO: Add sprite(s) for Mermaids.
      }
    }

    internal sealed class Wizard : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool HatJunimos { get; set; } = false;
      public bool ShoulderJunimos { get; set; } = false;
      public bool SpiritCreatures { get; set; } = false;
      public WizardMarriageMod MarriageMod { get; set; } = WizardMarriageMod.None;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        this.RegisterImmersionToken<SimpleImmersion>(() => this.Immersion);
        TokenRegistry.AddCompositeToken("WizardFamiliars", new() {
          ["HatJunimos"] = () => this.Immersion.IsFull() && this.HatJunimos,
          ["ShoulderJunimos"] = () => this.Immersion.IsFull() && this.ShoulderJunimos,
          ["SpiritCreatures"] = () => this.Immersion.IsFull() && this.SpiritCreatures
        });
        // TODO: Determine whether we should be smarter about the value we return for this token.
        TokenRegistry.AddEnumToken<WizardMarriageMod>("WizardMarriageMod", () => this.MarriageMod);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Spring_1";
      }
    }

    internal sealed class Witch : BaseCharacterSection {
      public SimpleVariant Variant { get; set; } = SimpleVariant.Off;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SimpleVariant>(() => this.Variant);
      }

      internal override Rectangle? GetPortraitRect() {
        return null; // No portrait available for Witch.
      }

      internal override Rectangle? GetSpriteRect() {
        return null; // TODO: Add sprite(s) for Witch.
      }
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
