using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class AlexEvelynGeorge : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Alex : BaseCharacterSection.Bachelorex<FamilyVariant> {
      public bool Tattoos { get; set; } = true;

      public override string GetPreviewOutfit() {
        return "Fall_1_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("JewishWeddingSuit") ? 6 : 5;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Alex>(
            "Tattoos", () => (this.Variant is FamilyVariant.Samoan) && this.Tattoos);
      }
    }

    internal sealed class Evelyn : BaseCharacterSection.Villager<FamilyVariant> {
      public override string GetPreviewOutfit() {
        return "Summer_1_Base";
      }
    }

    internal sealed class George : BaseCharacterSection.Villager<FamilyVariant> {
      public GeorgeBeard Beard { get; set; } = GeorgeBeard.Dynamic;

      public override string GetPreviewOutfit() {
        return "Fall_1_Rain";
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterEnumToken(
            "GeorgeBeard",
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
