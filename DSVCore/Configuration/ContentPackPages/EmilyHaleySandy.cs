using System.Collections.Generic;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class EmilyHaleySandy : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Emily : BaseCharacterSection.Bachelorex<FamilyVariant> {
      public bool Tattoos { get; set; } = true;

      public override string GetPreviewOutfit() {
        return "Summer_3_Base";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("UkrainianWeddingDress") ? 7 : 6;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterAutoNamedBoolToken<Emily>(
            "Tattoos", () => this.Immersion.IsFull() && this.Tattoos);
      }

      protected override IEnumerable<string> GetImageOverlayPaths(
          string imageDirectory, string variant, IDictionary<string, object?> ephemeralProperties) {
        if (ephemeralProperties.IsTrueValue(nameof(this.Tattoos))) {
          yield return $"Emily/{imageDirectory}/Tattoos/Emily_{this.GetPreviewOutfit()}.png";
        }
      }
    }

    internal sealed class Haley : BaseCharacterSection.Bachelorex<FamilyVariant> {
      public bool HairCuffs { get; set; } = false;
      public bool Piercings { get; set; } = false;
      public bool BlackCamera { get; set; } = false;

      public override string GetPreviewOutfit() {
        return "Spring_2_Sun";
      }

      public override int GetNumberOfWeddingOutfits() {
        return this.HasElahoOutfit("RussianWeddingDress") ? 7 : 6;
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterCompositeToken("HaleyAccessories", new() {
          ["Cuffs"] = () => (this.Variant is FamilyVariant.Black) && this.HairCuffs,
          ["Piercings"] = () => this.Immersion.IsNotUltralight() && this.Piercings,
          ["BlackCam"] = () => this.Immersion.IsNotUltralight() && this.BlackCamera
        });
      }
    }

    internal sealed class Sandy : BaseCharacterSection,
        IHasVariant<StandardVariant>, IHasImmersion<SimpleImmersion>, IHasCustomModImageDirectory {

      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool GiftTastesChange { get; set; } = true;

      public string GetPreviewOutfit() {
        return "Winter_2_Base";
      }

      protected override void RegisterExtraTokens(Integration contentPatcher) {
        contentPatcher.RegisterBoolToken(
            "SandyGiftTastesChange", () => this.Variant.IsModded() && this.GiftTastesChange);
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
