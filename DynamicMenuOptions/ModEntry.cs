using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal sealed class ModEntry : BaseMod {

  private const string DataAssetNameString = "Mods/DynamicMenuOptions";

  private IAssetName? DataAssetName;

  protected override void OnGameLaunched() {
    if (!this.TryIntegrateWithGMCM(out GenericModConfigMenuIntegration? gmcmIntegration)) {
      Log.Error("Could not retrieve the Generic Mod Config Menu API. This mod will be disabled.");
      return;
    }

    this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    this.Helper.Events.Content.AssetsInvalidated += this.OnAssetsInvalidated;

    gmcmIntegration.CreateSinglePageMenuBuilder()
        .AddStaticHeader("Hello World!")
        .AddStaticText("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod " +
            "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis " +
            "nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")
        .EndPage()
        .PublishMenu();
  }

  private void OnAssetRequested(object? sender, AssetRequestedEventArgs args) {
    if (args.Name.IsEquivalentTo(DataAssetNameString)) {
      this.DataAssetName = args.Name;
      args.LoadFrom(() => new Dictionary<string, object>(), AssetLoadPriority.Low);
    }
  }

  private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs args) {
    if ((this.DataAssetName != null) && args.Names.Contains(this.DataAssetName)) {
      // TODO: Track and/or update the set of managed/integrated mods.
    }
  }
}
