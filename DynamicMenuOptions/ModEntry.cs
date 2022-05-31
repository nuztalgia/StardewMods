using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal sealed class ModEntry : BaseMod {

  private const string DataAssetNameString = "Mods/DynamicMenuOptions";

  private Dictionary<string, MenuSpec.RawData> RawMenuSpecs =>
      this.Helper.GameContent.Load<Dictionary<string, MenuSpec.RawData>>(DataAssetNameString);

  private GenericModConfigMenuIntegration? ConfigMenu;
  private IAssetName? DataAssetName;

  protected override void OnModEntry() {
    ContentPatcherBridge.Initialize(this.ModManifest.UniqueID, this.OnContentPatcherInitialized);
  }

  protected override void OnGameLaunched() {
    if (ContentPatcherBridge.HasMissingMethods) {
      Log.Error(
          "Required Content Patcher method(s) could not be found. This mod will be disabled." +
          ContentPatcherBridge.MissingMethodText);
      return;
    }

    if (!this.TryIntegrateWithGMCM(out GenericModConfigMenuIntegration? gmcmIntegration)) {
      Log.Error("Could not retrieve the Generic Mod Config Menu API. This mod will be disabled.");
      return;
    }

    this.ConfigMenu = gmcmIntegration;

    this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    this.Helper.Events.Content.AssetsInvalidated += this.OnAssetsInvalidated;

    _ = this.RawMenuSpecs; // Trigger an asset request so EditData patches can be applied/evaluated.
  }

  private void OnAssetRequested(object? sender, AssetRequestedEventArgs args) {
    if (args.Name.IsEquivalentTo(DataAssetNameString)) {
      this.DataAssetName = args.Name;
      args.LoadFrom(() => new Dictionary<string, MenuSpec.RawData>(), AssetLoadPriority.Low);
    }
  }

  private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs args) {
    if ((this.DataAssetName != null) && args.Names.Contains(this.DataAssetName)) {
      ContentPatcherBridge.UpdateManagedMods(modIds: this.RawMenuSpecs.Keys);
    }
  }

  private void OnContentPatcherInitialized() {
    if (this.ConfigMenu != null) {
      // TODO: Add the managed mods' configs to GMCM as their own menus (not in this mod's menu).
      IConfigPageBuilder pageBuilder = this.ConfigMenu.CreateSinglePageMenuBuilder();

      foreach (string modId in this.RawMenuSpecs.Keys) {
        pageBuilder.AddStaticHeader(modId);
        foreach (ConfigFieldData configFieldData in ContentPatcherBridge.GetModConfig(modId)) {
          pageBuilder.AddStaticText($"  > {configFieldData.Key}");
        }
      }
      pageBuilder.EndPage().PublishMenu();
    }
  }
}
