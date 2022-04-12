using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class ShaneJasMarnie : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Shane : BaseBachelorexSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public int WeddingOutfit { get; set; } = 1;
      public ShaneSelfCare SelfCare { get; set; } = ShaneSelfCare.Dynamic;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(tokenMap, nameof(this.SelfCare));
      }

      protected override int GetNumberOfWeddingOutfits() {
        return HasElahoMod("ShaneGeorgianWeddingSuit") ? 6 : 5;
      }
    }

    internal sealed class Jas : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
    }

    internal sealed class Marnie : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;
      public bool SpriteSmile { get; set; } = true;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(
            tokenMap, nameof(this.SpriteSmile), customSuffix: "CharacterSmile",
            valueIfTrue: "Smile", valueIfFalse: "NoSmile");
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
