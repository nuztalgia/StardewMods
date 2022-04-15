namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class EmilyHaleySandy : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Emily : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Tattoos { get; set; } = true;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<FamilyVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        this.RegisterAutoNamedBoolToken("Tattoos", () => this.Immersion.IsFull() && this.Tattoos);
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("EmilyUkrainianWeddingDress") ? 7 : 6;
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Spring_1_Sun";
      }
    }

    internal sealed class Haley : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool HairCuffs { get; set; } = false;
      public bool Piercings { get; set; } = false;
      public bool BlackCamera { get; set; } = false;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<FamilyVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        TokenRegistry.AddCompositeToken("HaleyAccessories", new() {
          ["Cuffs"] = () => (this.Variant is FamilyVariant.Black) && this.HairCuffs,
          ["Piercings"] = () => this.Immersion.IsNotUltralight() && this.Piercings,
          ["BlackCam"] = () => this.Immersion.IsNotUltralight() && this.BlackCamera
        });
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("HaleyRussianWeddingDress") ? 7 : 6;
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Spring_2_Sun";
      }
    }

    internal sealed class Sandy : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool GiftTastesChange { get; set; } = true;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        this.RegisterImmersionToken<SimpleImmersion>(() => this.Immersion);
        TokenRegistry.AddBoolToken(
            "SandyGiftTastesChange", () => this.Variant.IsModded() && this.GiftTastesChange);
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Winter_2_Base";
      }
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
