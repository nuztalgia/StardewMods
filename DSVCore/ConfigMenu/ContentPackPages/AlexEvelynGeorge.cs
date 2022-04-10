namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AlexEvelynGeorge : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Alex : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
      public int WeddingOutfit { get; set; } = 1;
      public bool Tattoos { get; set; } = true;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("AlexJewishWeddingSuit") ? 6 : 5;
      }
    }

    internal sealed class Evelyn : BaseCharacterSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class George : BaseCharacterSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public GeorgeBeard Beard { get; set; } = GeorgeBeard.Dynamic;
    }
  }

  internal enum FamilyVariant {
    Mexican,
    Samoan,
    Vanilla,
    Off
  }

  internal enum GeorgeBeard {
    Beard,
    NoBeard,
    Dynamic
  }

  public Sections.Alex Alex { get; set; } = new();
  public Sections.Evelyn Evelyn { get; set; } = new();
  public Sections.George George { get; set; } = new();
}