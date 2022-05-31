using System.Collections.Immutable;
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
      ConfigMenuBuilder.PopulateConfigMenu(
          this.ConfigMenu,
          this.RawMenuSpecs.ToImmutableDictionary(
              kvp => kvp.Key, // Mod ID.
              kvp => CreateModMenuData(kvp.Key, kvp.Value)));
    }

    static ConfigMenuBuilder.ModMenuData CreateModMenuData(string modId, MenuSpec.RawData rawSpec) {
      return new(
          Manifest: ModRegistryUtils.GetModManifest(modId)!,
          ConfigFields: ContentPatcherBridge.GetModConfig(modId),
          MenuSpec: MenuSpec.ResolveData(modId, rawSpec));
    }
  }
}
