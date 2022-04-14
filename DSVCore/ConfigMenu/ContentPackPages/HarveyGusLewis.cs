namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class HarveyGusLewis : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Harvey : BaseBachelorexSection {
      public HarveyVariant Variant { get; set; } = HarveyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool GiftTastesChange { get; set; } = true;
      public bool SpriteMustache { get; set; } = false;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<HarveyVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        TokenRegistry.AddBoolToken(
            "HarveyGiftTastesChange",
            () => (this.Variant is HarveyVariant.ModdedSikh) && this.GiftTastesChange);
        TokenRegistry.AddBoolToken(
            "HarveyCharacterMustache",
            () => (this.Variant is HarveyVariant.Vanilla) && this.SpriteMustache,
            autoValueString: "Mustache");
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("HarveyHungarianWeddingSuit") ? 5 : 4;
      }
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
}
