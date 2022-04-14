namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AlexEvelynGeorge : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Alex : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Tattoos { get; set; } = true;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<FamilyVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        this.RegisterAutoNamedBoolToken(
            "Tattoos", () => (this.Variant is FamilyVariant.Samoan) && this.Tattoos);
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("AlexJewishWeddingSuit") ? 6 : 5;
      }
    }

    internal sealed class Evelyn : BaseCharacterSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<FamilyVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
      }
    }

    internal sealed class George : BaseCharacterSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public GeorgeBeard Beard { get; set; } = GeorgeBeard.Dynamic;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<FamilyVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
        TokenRegistry.AddEnumToken<GeorgeBeard>("GeorgeBeard",
            () => this.Immersion.IsNotUltralight() ? this.Beard : GeorgeBeard.NoBeard);
      }
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
