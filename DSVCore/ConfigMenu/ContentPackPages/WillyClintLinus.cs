namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class WillyClintLinus : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Willy : BaseCharacterSection {
      public WillyVariant Variant { get; set; } = WillyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Clint : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public bool Scar { get; set; } = false;
    }

    internal sealed class Linus : BaseCharacterSection {
      public LinusVariant Variant { get; set; } = LinusVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
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
