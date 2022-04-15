namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class LeahElliott : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Leah : BaseBachelorexSection {
      public LeahVariant Variant { get; set; } = LeahVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<LeahVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("LeahPolishWeddingOutfits") ? 7 : 5;
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Fall_2_Base";
      }
    }

    internal sealed class Elliott : BaseBachelorexSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("ElliottScottishWeddingKilt") ? 6 : 5;
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Fall_1_Base";
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
