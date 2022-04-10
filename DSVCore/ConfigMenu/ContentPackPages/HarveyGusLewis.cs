namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class HarveyGusLewis : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Harvey : BaseBachelorexSection {
      public HarveyVariant Variant { get; set; } = HarveyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
      public int WeddingOutfit { get; set; } = 1;
      public bool GiftTastesChange { get; set; } = true;
      public bool SpriteMustache { get; set; } = false;
    }

    internal sealed class Gus : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Lewis : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }
  }

  internal enum HarveyVariant {
    ModdedSikh,
    ModdedNonSikh,
    Vanilla,
    Off
  }

  public Sections.Harvey Harvey { get; set; } = new();
  public Sections.Gus Gus { get; set; } = new();
  public Sections.Lewis Lewis { get; set; } = new();
  public CompatSections.RidgesideVillage.Lenny Lenny { get; set; } = new();
}
