using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.Pages;

internal sealed class MarlonGuntherMorris : BaseContentPackPage {

  internal static class Sections {
    internal sealed class Marlon : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
    }

    internal sealed class Gunther : BaseCharacterSection {
      public StandardVariant Variant { get; set; } = StandardVariant.Vanilla;
      public bool AlternateCecily { get; set; } = true;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(tokenMap, nameof(this.AlternateCecily), customPrefix: "");
      }
    }

    internal sealed class Morris : BaseCharacterSection {
      public bool SeasonalOutfits { get; set; } = true;

      internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
        base.AddTokens(tokenMap);
        this.AddTokenByProperty(
            tokenMap, nameof(this.SeasonalOutfits), customSuffix: "Variant",
            valueIfTrue: "Vanilla", valueIfFalse: "Off");
      }
    }
  }

  public Sections.Marlon Marlon { get; set; } = new();
  public Sections.Gunther Gunther { get; set; } = new();
  public Sections.Morris Morris { get; set; } = new();
}
