namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class EmilyHaleySandy : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Emily : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Tattoos { get; set; } = true;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("EmilyUkrainianWeddingDress") ? 7 : 6;
      }
    }

    internal sealed class Haley : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool HairCuffs { get; set; } = false;
      public bool Piercings { get; set; } = false;
      public bool BlackCamera { get; set; } = false;

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("HaleyRussianWeddingDress") ? 7 : 6;
      }
    }

    internal sealed class Sandy : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool GiftTastesChange { get; set; } = true;
    }
  }

  internal enum FamilyVariant {
    Black,
    Romani,
    Vanilla,
    Off
  }

  public Sections.Emily Emily { get; set; } = new();
  public Sections.Haley Haley { get; set; } = new();
  public Sections.Sandy Sandy { get; set; } = new();
}
