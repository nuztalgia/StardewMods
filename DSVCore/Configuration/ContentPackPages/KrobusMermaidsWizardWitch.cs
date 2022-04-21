using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class KrobusMermaidsWizardWitch : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Krobus : BaseCharacterSection,
        IHasVariant<SimpleVariant>, IHasImmersion<SimpleImmersion>, IHasCustomModImagePath {

      private static readonly Rectangle[][] SpriteRect = Wrap(new Rectangle(0, 0, 16, 24));

      public SimpleVariant Variant { get; set; } = SimpleVariant.Modded;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;

      string IHasCustomModImagePath.GetModImagePath(string imageDirectory) {
        return $"Krobus/{imageDirectory}/Krobus_1_Snow.png";
      }

      internal override ImagePreviews.GetImageRects? GetSpriteRectsDelegate() {
        return _ => SpriteRect;
      }
    }

    internal sealed class Mermaids : BaseCharacterSection,
        IHasCustomDisplayName, IHasCustomPreviewTooltip {

      private const int NumberOfVariants = 4;
      private const string ModImagePath = "Mermaids/Mermaid{0}/Mermaid{0}_{1}.png";

      private static readonly Rectangle[][] ModSpriteRects =
          CopyForAllVariants(new Rectangle[] { new(28, 0, 25, 35), new(0, 0, 25, 35) });

      private static readonly string[] RandomUppers = new[] {
          string.Format(ModImagePath, 'C', "Top1"), string.Format(ModImagePath, 'C', "Top2"),
          string.Format(ModImagePath, 'C', "Top3"), string.Format(ModImagePath, 'C', "Top4")
      };
      private static readonly string[] RandomLowers = new[] {
          string.Format(ModImagePath, 'E', "Tail1"), string.Format(ModImagePath, 'E', "Tail2"),
          string.Format(ModImagePath, 'E', "Tail3"), string.Format(ModImagePath, 'E', "Tail4")
      };

      private static readonly string[] VanillaUppers =
          CopyForAllVariants(string.Format(ModImagePath, 'C', "Top1"));
      private static readonly string[] VanillaLowers =
          CopyForAllVariants(string.Format(ModImagePath, 'E', "Tail1"));

      private static readonly Random Randomizer = new();

      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;

      string IHasCustomDisplayName.GetDisplayName() {
        return I18n.Character_Mermaids();
      }

      internal override string[][] GetModImagePaths(
          string _, IDictionary<string, object?> ephemeralProperties) {
        ephemeralProperties.TryGetValue(nameof(this.Randomization), out object? value);
        Enum.TryParse(typeof(MermaidRandomization), value?.ToString(), out object? enumValue);
        return enumValue switch {
          MermaidRandomization.RandomBoth => GenerateMermaids(true, true),
          MermaidRandomization.RandomUpper => GenerateMermaids(true, false),
          MermaidRandomization.RandomLower => GenerateMermaids(false, true),
          MermaidRandomization.Light => Combine(RandomLowers, RandomUppers).ToArray(),
          MermaidRandomization.Off => Wrap(VanillaLowers[0], VanillaUppers[0]),
          _ => Wrap<string>()
        };
      }

      internal override ImagePreviews.GetImageRects? GetPortraitRectsDelegate() {
        return _ => ModSpriteRects; // These are more like sprites, but should use portrait scaling.
      }

      internal override ImagePreviews.GetImageRects? GetSpriteRectsDelegate() {
        return null; // See comment above.
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterEnumToken("Mermaids", () => this.Randomization);
      }

      private static string[][] GenerateMermaids(bool randomUppers, bool randomLowers) {
        string[] uppers = randomUppers ? GetShuffledArray(RandomUppers) : VanillaUppers;
        string[] lowers = randomLowers ? GetShuffledArray(RandomLowers) : VanillaLowers;
        return Combine(lowers, uppers).ToArray(); // Uppers listed 2nd to be drawn on top of lowers.
      }

      private static IEnumerable<string[]> Combine(string[] firsts, string[] seconds) {
        foreach (var (first, second) in firsts.Zip(seconds)) {
          yield return new[] { first, second };
        }
      }

      private static string[] GetShuffledArray(string[] array) {
        return array.OrderBy(item => Randomizer.Next()).ToArray();
      }

      private static T[] CopyForAllVariants<T>(T items) {
        return Enumerable.Repeat(items, NumberOfVariants).ToArray();
      }
    }

    internal sealed class Wizard : BaseCharacterSection,
        IHasVariant<StandardVariant>, IHasImmersion<SimpleImmersion>,
        IHasCustomDisplayName, IHasCustomModImageDirectory {

      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool HatJunimos { get; set; } = false;
      public bool ShoulderJunimos { get; set; } = false;
      public bool SpiritCreatures { get; set; } = false;
      public WizardMarriageMod MarriageMod { get; set; } = WizardMarriageMod.None;

      string IHasCustomDisplayName.GetDisplayName() {
        return I18n.Character_Wizard();
      }

      string IHasVariant.GetPreviewOutfit() {
        return "Spring_1";
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterCompositeToken("WizardFamiliars", new() {
          ["HatJunimos"] = () => this.Immersion.IsFull() && this.HatJunimos,
          ["ShoulderJunimos"] = () => this.Immersion.IsFull() && this.ShoulderJunimos,
          ["SpiritCreatures"] = () => this.Immersion.IsFull() && this.SpiritCreatures
        });
        // TODO: Determine whether we should be smarter about the value we return for this token.
        contentPatcher.RegisterEnumToken("WizardMarriageMod", () => this.MarriageMod);
      }
    }

    internal sealed class Witch : BaseCharacterSection,
        IHasVariant<SimpleVariant>, IHasCustomDisplayName,
        IHasCustomPreviewTooltip, IHasCustomModImagePath {

      private static readonly Rectangle[][] GameSpriteRect = Wrap(new Rectangle(276, 1885, 35, 30));
      private static readonly Rectangle[][] ModSpriteRect = Wrap(new Rectangle(0, 0, 35, 30));

      public SimpleVariant Variant { get; set; } = SimpleVariant.Off;

      string IHasCustomDisplayName.GetDisplayName() {
        return I18n.Character_Witch();
      }

      string IHasCustomModImagePath.GetModImagePath(string _) {
        return "Witch/CursorsWitch.png";
      }

      internal override string[][] GetGameImagePaths(string _) {
        return Wrap("LooseSprites/Cursors");
      }

      internal override ImagePreviews.GetImageRects? GetPortraitRectsDelegate() {
        return null; // No portrait available for the Witch.
      }

      internal override ImagePreviews.GetImageRects? GetSpriteRectsDelegate() {
        return source => (source == ContentSource.GameContent) ? GameSpriteRect : ModSpriteRect;
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
