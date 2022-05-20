namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackPage : BaseMenuPage {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";

  private readonly string ContentPackId;

  internal BaseContentPackPage() {
    this.ContentPackId = $"{RootModId}.{this.Name}";
  }

  internal CharacterConfigState.LoadImage GetImageLoader() {
    return imagePath => ModRegistryUtils.LoadImageFromContentPack(this.ContentPackId, imagePath);
  }

  internal override string GetDisplayName() {
    return I18nHelper.GetStringByKeyName($"Page_{this.Name}") ?? this.Name;
  }

  internal override bool IsAvailable() {
    return ModRegistryUtils.IsLoaded(this.ContentPackId);
  }
}
