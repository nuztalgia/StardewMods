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

      internal override CharacterConfigState.GetImageRects? GetSpriteRectsDelegate() {
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
          ModImagePath.Format('C', "Top1"), ModImagePath.Format('C', "Top2"),
          ModImagePath.Format('C', "Top3"), ModImagePath.Format('C', "Top4")
      };
      private static readonly string[] RandomLowers = new[] {
          ModImagePath.Format('E', "Tail1"), ModImagePath.Format('E', "Tail2"),
          ModImagePath.Format('E', "Tail3"), ModImagePath.Format('E', "Tail4")
      };

      private static readonly string[] VanillaUppers =
          CopyForAllVariants(ModImagePath.Format('C', "Top1"));
      private static readonly string[] VanillaLowers =
          CopyForAllVariants(ModImagePath.Format('E', "Tail1"));

      public MermaidRandomization Randomization { get; set; } = MermaidRandomization.RandomBoth;

      string IHasCustomDisplayName.GetDisplayName() {
        return I18n.Character_Mermaids();
      }

      internal override string[][] GetModImagePaths(
          string _, IDictionary<string, object?> ephemeralState) {
        ephemeralState.TryGetValue(nameof(this.Randomization), out object? value);
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

      internal override CharacterConfigState.GetImageRects? GetPortraitRectsDelegate() {
        return _ => ModSpriteRects; // These are more like sprites, but should use portrait scaling.
      }

      internal override CharacterConfigState.GetImageRects? GetSpriteRectsDelegate() {
        return null; // See comment above.
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterEnumToken("Mermaids", () => this.Randomization);
      }

      private static string[][] GenerateMermaids(bool randomUppers, bool randomLowers) {
        string[] uppers = randomUppers ? RandomUppers.Shuffle().ToArray() : VanillaUppers;
        string[] lowers = randomLowers ? RandomLowers.Shuffle().ToArray() : VanillaLowers;
        return Combine(lowers, uppers).ToArray(); // Uppers listed 2nd to be drawn on top of lowers.
      }

      private static IEnumerable<string[]> Combine(string[] firsts, string[] seconds) {
        foreach (var (first, second) in firsts.Zip(seconds)) {
          yield return new[] { first, second };
        }
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

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        if ((imageDirectory == CharacterConfigState.PortraitsDirectory) && ephemeralState.Any()) {
          string pathPrefix = this.GetModImagePath(imageDirectory, variant) + "WizardFamiliars";

          if (ephemeralState.IsTrueValue(nameof(this.HatJunimos))) {
            yield return FormatImagePath(nameof(this.HatJunimos), "Hat_Junimo", "Pink");
          }

          if (ephemeralState.IsTrueValue(nameof(this.ShoulderJunimos))) {
            yield return FormatImagePath(nameof(this.ShoulderJunimos), "Shoulder_Junimo", "Spring");
          }

          if (ephemeralState.IsTrueValue(nameof(this.SpiritCreatures))) {
            yield return FormatImagePath(nameof(this.SpiritCreatures), "Spirit_Creature", "Solar");
          }

          string FormatImagePath(string propertyName, string filePrefix, string fileSuffix) {
            return $"{pathPrefix}/{propertyName}/{filePrefix}_{fileSuffix}.png";
          }
        }
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

      internal override CharacterConfigState.GetImageRects? GetPortraitRectsDelegate() {
        return source => (source == ContentSource.GameContent) ? GameSpriteRect : ModSpriteRect;
      }

      internal override CharacterConfigState.GetImageRects? GetSpriteRectsDelegate() {
        return null; // The witch only really has sprites, but should use portrait scaling (above).
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
