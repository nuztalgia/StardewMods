using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class KrobusMermaidsWizardWitch : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Krobus : BaseCharacterSection {
      private static readonly Rectangle SpriteRect = new(0, 0, 16, 24);  // He's a smol boi.

      public SimpleVariant Variant { get; set; } = SimpleVariant.Modded;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SimpleVariant>(() => this.Variant);
        this.RegisterImmersionToken<SimpleImmersion>(() => this.Immersion);
      }

      internal override ImagePreviews.GetImageRect? GetSpriteRectDelegate() {
        return _ => SpriteRect;
      }

      protected override string GetModImagePath(string imageDirectory, string _) {
        return $"Krobus/{imageDirectory}/Krobus_1_Snow.png";
      }
    }

    internal sealed class Mermaids : BaseCharacterSection {
      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;

      internal override void RegisterTokens() {
        TokenRegistry.AddEnumToken<MermaidRandomization>("Mermaids", () => this.Randomization);
      }

      internal override ImagePreviews.GetImageRect? GetPortraitRectDelegate() {
        return null; // No portrait available for the Mermaids.
      }

      internal override ImagePreviews.GetImageRect? GetSpriteRectDelegate() {
        return null; // TODO: Add sprite(s) for the Mermaids.
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
      private static readonly Rectangle GameSpriteRect = new(276, 1885, 35, 30);
      private static readonly Rectangle ModSpriteRect =
          new(0, 0, GameSpriteRect.Width, GameSpriteRect.Height);

      public SimpleVariant Variant { get; set; } = SimpleVariant.Off;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<SimpleVariant>(() => this.Variant);
      }

      internal override ImagePreviews.GetImageRect? GetPortraitRectDelegate() {
        return null; // No portrait available for the Witch.
      }

      internal override ImagePreviews.GetImageRect? GetSpriteRectDelegate() {
        return source => (source == ContentSource.GameContent) ? GameSpriteRect : ModSpriteRect;
      }

      internal override string GetGameImagePath(string _) {
        return "LooseSprites/Cursors";
      }

      protected override string GetModImagePath(string _, string __) {
        return "Witch/CursorsWitch.png";
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
