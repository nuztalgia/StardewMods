namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class MarlonGuntherMorris : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Marlon : BaseCharacterSection,
        IHasVariant<StandardVariant>, IHasCustomModImageDirectory, IHasCustomPreviewTooltip {

      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      public string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }

    internal sealed class Gunther : BaseCharacterSection,
        IHasVariant<StandardVariant>, IHasCustomModImageDirectory, IHasCustomPreviewTooltip {

      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;

      public string GetPreviewOutfit() {
        return "Fall_1_Base";
      }
    }

    internal sealed class Morris : BaseCharacterSection, IHasCustomPreviewTooltip {
      public bool SeasonalOutfits { get; set; } = true;

      internal override string[][] GetModImagePaths(
          string imageDirectory, IDictionary<string, object?> ephemeralState) {
        return ephemeralState.IsTrueValue(nameof(this.SeasonalOutfits))
            ? Wrap($"Morris/{imageDirectory}/Morris_Fall_1_Base.png")
            : Wrap<string>();
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "MorrisVariant",
            () => this.SeasonalOutfits,
            valueIfTrue: nameof(StandardVariant.Vanilla),
            valueIfFalse: nameof(StandardVariant.Off));
      }
    }
  }

  public Sections.Marlon Marlon { get; set; } = new();
  public Sections.Gunther Gunther { get; set; } = new();
  public Sections.Morris Morris { get; set; } = new();
}
