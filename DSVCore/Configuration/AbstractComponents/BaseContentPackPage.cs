namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackPage : BaseMenuPage {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";

  private readonly string ContentPackId;

  internal BaseContentPackPage() {
    this.ContentPackId = $"{RootModId}.{this.Name}";
  }

  internal override string GetDisplayName() {
    return Globals.GetI18nString($"Page_{this.Name}") ?? this.Name;
  }

  internal override bool IsAvailable() {
    return Globals.ModRegistry.IsLoaded(this.ContentPackId);
  }
}
