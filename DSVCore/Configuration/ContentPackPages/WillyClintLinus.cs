namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class WillyClintLinus : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Willy : BaseCharacterSection.Villager<WillyVariant>,
        IHasCustomModImageDirectory {
      public override string GetPreviewOutfit() {
        return "Spring_1_Base";
      }
    }

    internal sealed class Clint : BaseCharacterSection.Villager<StandardVariant>,
        IHasCustomModImageDirectory {
      public bool Scar { get; set; } = false;

      public override string GetPreviewOutfit() {
        return "Winter_2_Base";
      }

      protected override void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Clint>("Scar", () => this.Scar);
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralState) {
        if (ephemeralState.IsTrueValue(nameof(this.Scar))) {
          yield return $"Clint/{imageDirectory}/Default/Clint_Scar.png";
        }
      }
    }

    internal sealed class Linus : BaseCharacterSection.Villager<LinusVariant>,
        IHasCustomModImageDirectory {
      public override string GetPreviewOutfit() {
        return "Spring_1_Sun";
      }

      string IHasCustomModImageDirectory.GetDirectory(string _) {
        return "Default";
      }
    }
  }

  internal enum WillyVariant {
    Tongan,
    TonganDisabled,
    Disabled,
    Vanilla,
    Off
  }

  internal enum LinusVariant {
    Vanilla,
    Off
  }

  public Sections.Willy Willy { get; set; } = new();
  public Sections.Clint Clint { get; set; } = new();
  public Sections.Linus Linus { get; set; } = new();
}
