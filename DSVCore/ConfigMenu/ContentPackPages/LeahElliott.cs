namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class LeahElliott : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Leah : BaseBachelorexSection {
      public LeahVariant Variant { get; set; } = LeahVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
      public int WeddingOutfit { get; set; } = 1;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("LeahPolishWeddingOutfits") ? 7 : 5;
      }
    }

    internal sealed class Elliott : BaseBachelorexSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
      public int WeddingOutfit { get; set; } = 1;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("ElliottScottishWeddingKilt") ? 6 : 5;
      }
    }
  }

  internal enum LeahVariant {
    Native,
    Butch,
    Vanilla,
    Off
  }

  public Sections.Leah Leah { get; set; } = new();
  public Sections.Elliott Elliott { get; set; } = new();
}
