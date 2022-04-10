using System.Collections.Generic;
using System.Linq;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class CoreAndCompatPage : BaseMenuPage {

  public CoreOptionsSection CoreOptions { get; set; } = new();
  public CompatSections.FlowerQueensCrown FlowerQueensCrown { get; set; } = new();
  public CompatSections.PlatonicPAF PlatonicPartnersAndFriendships { get; set; } = new();
  public CompatSections.LookingForLove LookingForLove { get; set; } = new();

  internal override string GetDisplayName() {
    return I18n.Page_CoreAndCompat();
  }

  internal override bool IsAvailable() {
    // This page is part of the core mod (a.k.a. this mod), so it's always available.
    return true;
  }

  internal IEnumerable<BaseMenuSection> GetCompatSections() {
    return this.GetAllSections().Where(
        section => section.IsAvailable() && section is BaseCompatSection);
  }
}
