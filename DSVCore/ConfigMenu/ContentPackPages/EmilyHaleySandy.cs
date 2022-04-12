using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class EmilyHaleySandy : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Emily : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Tattoos { get; set; } = true;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(
            tokenMap, nameof(this.Tattoos), valueIfTrue: "Tattoos", valueIfFalse: "NoTattoos");
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("EmilyUkrainianWeddingDress") ? 7 : 6;
      }
    }

    internal sealed class Haley : BaseBachelorexSection {
      public FamilyVariant Variant { get; set; } = FamilyVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public bool Cuffs { get; set; } = false;
      public bool Piercings { get; set; } = false;
      public bool BlackCam { get; set; } = false;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        tokenMap.Add("HaleyAccessories", () => this.GetCombinedTokenValues(
            nameof(this.Cuffs), nameof(this.Piercings), nameof(this.BlackCam)));
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("HaleyRussianWeddingDress") ? 7 : 6;
      }
    }

    internal sealed class Sandy : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public SimpleImmersion Immersion { get; set; } = SimpleImmersion.Full;
      public bool GiftTastesChange { get; set; } = true;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(tokenMap, nameof(this.GiftTastesChange));
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
