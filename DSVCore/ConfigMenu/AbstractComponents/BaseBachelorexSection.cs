using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

// "Bachelorex" is a gender-neutral term for "Bachelor" or "Bachelorette".
internal abstract class BaseBachelorexSection : BaseCharacterSection {

  private const string PropertyNameWeddingOutfit = "WeddingOutfit";

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    base.AddTokens(tokenMap);
    this.AddTokenByProperty(tokenMap, PropertyNameWeddingOutfit,
                            customPrefix: PropertyNameWeddingOutfit, customSuffix: this.Name);
  }

  internal override int? GetMinValue(PropertyInfo property) {
    return (property.Name == PropertyNameWeddingOutfit)
            ? 1 // The wedding outfts for all characters are indexed starting at 1.
            : base.GetMinValue(property);
  }

  internal override int? GetMaxValue(PropertyInfo property) {
    return (property.Name == PropertyNameWeddingOutfit)
            ? this.GetNumberOfWeddingOutfits()
            : base.GetMaxValue(property);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return (property.Name == PropertyNameWeddingOutfit)
            ? this.FormatCharacterString(I18n.Tooltip_WeddingOutfit)
            : base.GetTooltip(property);
  }

  protected abstract int GetNumberOfWeddingOutfits();

  protected static bool HasElahoMod(string modName) {
    return Globals.ModRegistry.IsLoaded($"Elaho.{modName}");
  }
}
