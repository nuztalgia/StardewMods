using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

// "Bachelorex" is a gender-neutral term for "Bachelor" or "Bachelorette".
internal abstract class BaseBachelorexSection : BaseCharacterSection {

  private const string WeddingOutfitPropertyName = "WeddingOutfit";

  // Subclasses should override this method to register their Variant and any other tokens. In doing
  // so, they should call base() to include the Immersion and WeddingOutfit tokens registered here.
  internal override void RegisterTokens() {
    this.TryRegisterStandardImmersionTokenUsingReflection();

    // TODO: Properly validate the WeddingOutfit token for every bachelorex.
    if (this.TryGetTokenProperty(WeddingOutfitPropertyName, out PropertyInfo? property)) {
      (int min, int max) = this.GetValueRange(property);
      TokenRegistry.AddIntToken(
          this.Name + property.Name, () => (int) property.GetValue(this)!, min, max);
    }
  }

  internal override (int min, int max) GetValueRange(PropertyInfo property) {
    return (property.Name == WeddingOutfitPropertyName) 
        ? (min: 1, max: this.GetNumberOfWeddingOutfits()) // Outfts are indexed starting at 1.
        : base.GetValueRange(property);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return (property.Name == WeddingOutfitPropertyName)
        ? this.FormatCharacterString(I18n.Tooltip_WeddingOutfit)
        : base.GetTooltip(property);
  }

  protected abstract int GetNumberOfWeddingOutfits();

  protected static bool HasElahoMod(string modName) {
    return Globals.ModRegistry.IsLoaded($"Elaho.{modName}");
  }
}
