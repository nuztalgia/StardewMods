namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class WillyClintLinus : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Willy : BaseCharacterSection {
      public WillyVariant Variant { get; set; } = WillyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<WillyVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Spring_1_Base";
      }
    }

    internal sealed class Clint : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public bool Scar { get; set; } = false;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
        this.RegisterAutoNamedBoolToken("Scar", () => this.Scar);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Winter_2_Base";
      }
    }

    internal sealed class Linus : BaseCharacterSection {
      public LinusVariant Variant { get; set; } = LinusVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<LinusVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
      }

      protected override string GetPreviewImagePath(string imageDirectory, string _) {
        return $"Linus/{imageDirectory}/Default/Linus_Spring_1_Sun";
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
