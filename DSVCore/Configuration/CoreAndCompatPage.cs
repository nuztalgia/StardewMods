namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class CoreAndCompatPage : BaseMenuPage {

  public CoreOptionsSection CoreOptions { get; set; } = new();
  public CompatSections.FlowerQueensCrown FlowerQueensCrown { get; set; } = new();
  public CompatSections.RidgesideVillage RidgesideVillage { get; set; } = new();
  public CompatSections.PlatonicPAF PlatonicPartnersAndFriendships { get; set; } = new();
  public CompatSections.LookingForLove LookingForLove { get; set; } = new();
  public CompatSections.MakeGuntherReal MakeGuntherReal { get; set; } = new();

  internal override string GetDisplayName() {
    return I18n.Page_CoreAndCompat();
  }

  internal override bool IsAvailable() {
    // This page is part of the core mod (a.k.a. this mod), so it's always available.
    return true;
  }

  internal IEnumerable<BaseCompatSection> GetAvailableCompatSections() {
    return this.GetAllSections()
        .Where(section => section.IsAvailable())
        .OfType<BaseCompatSection>();
  }
}
