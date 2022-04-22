using Nuztalgia.StardewMods.Common.ModRegistry;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackPage : BaseMenuPage {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";

  private readonly string ContentPackId;

  internal BaseContentPackPage() {
    this.ContentPackId = $"{RootModId}.{this.Name}";
  }

  internal ImagePreviewOptions.LoadImage GetImageLoader() {
    return imagePath => ModRegistry.LoadImageFromContentPack(this.ContentPackId, imagePath);
  }

  internal override string GetDisplayName() {
    return Globals.GetI18nString($"Page_{this.Name}") ?? this.Name;
  }

  internal override bool IsAvailable() {
    return ModRegistry.IsLoaded(this.ContentPackId);
  }
}
