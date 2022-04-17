namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class ShaneJasMarnie : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Shane : BaseBachelorexSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public ShaneSelfCare SelfCare { get; set; } = ShaneSelfCare.Dynamic;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        base.RegisterTokens(); // Register Immersion and WeddingOutfit tokens.
        // TODO: Determine what should happen if Immersion is Ultralight and SelfCare is Dynamic.
        TokenRegistry.AddEnumToken<ShaneSelfCare>("ShaneSelfCare", () => this.SelfCare);
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("ShaneGeorgianWeddingSuit") ? 6 : 5;
      }

      protected override string GetModImagePath(string imageDirectory, string variant) {
        string outfitPath = (this.SelfCare == ShaneSelfCare.Messy)
                            ? "Messy/Shane_Fall_1_Sun"
                            : "Neat/Shane_Fall_2_Sun";
        return $"Shane/{imageDirectory}/{variant}/{outfitPath}.png";
      }
    }

    internal sealed class Jas : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = false;
        return "Fall_1_Base";
      }
    }

    internal sealed class Marnie : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public bool SpriteSmile { get; set; } = true;

      internal override void RegisterTokens() {
        this.RegisterVariantToken<StandardVariant>(() => this.Variant);
        this.RegisterImmersionToken<StandardImmersion>(() => this.Immersion);
        TokenRegistry.AddBoolToken(
            "MarnieCharacterSmile",
            () => (this.Variant is StandardVariant.Vanilla) && this.SpriteSmile,
            autoValueString: "Smile");
      }

      protected override string GetPreviewOutfit(out bool hasDefaultDirectory) {
        hasDefaultDirectory = true;
        return "Spring_1_Sun";
      }
    }
  }

  internal enum ShaneSelfCare {
    Neat,
    Messy,
    Dynamic
  }

  public Sections.Shane Shane { get; set; } = new();
  public Sections.Jas Jas { get; set; } = new();
  public Sections.Marnie Marnie { get; set; } = new();
}
