namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class MarlonGuntherMorris : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Marlon : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
    }

    internal sealed class Gunther : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public bool AlternateCecily { get; set; } = true;
    }

    internal sealed class Morris : BaseCharacterSection {
      public bool SeasonalOutfits { get; set; } = true;
    }
  }

  public Sections.Marlon Marlon { get; set; } = new();
  public Sections.Gunther Gunther { get; set; } = new();
  public Sections.Morris Morris { get; set; } = new();
}
