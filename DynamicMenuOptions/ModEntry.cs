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
        pageBuilder.AddStaticHeader(ModRegistryUtils.GetModManifest(modId)?.Name ?? modId);
        foreach (ConfigFieldData configField in ContentPatcherBridge.GetModConfig(modId)) {
          AddConfigFieldWidget(pageBuilder, configField);
        }
      }
      pageBuilder.EndPage().PublishMenu();
    }

    static void AddConfigFieldWidget(IConfigPageBuilder pageBuilder, ConfigFieldData configField) {
      string name = configField.Key;
      switch (configField.GetDataForUI()) {
        case ConfigFieldData.CheckboxData checkboxData:
          pageBuilder.AddCheckboxOption(name,
              loadValue: () => checkboxData.CurrentValue,
              saveValue: (value) => Log.Trace($"TODO: Save value '{value}' for checkbox {name}."));
          return;
        case ConfigFieldData.DropdownData dropdownData:
          pageBuilder.AddDropdownOption(name,
              allowedValues: dropdownData.AllowedValues,
              loadValue: () => dropdownData.CurrentValue,
              saveValue: (value) => Log.Trace($"TODO: Save value '{value}' for dropdown {name}."));
          return;
        case ConfigFieldData.SliderData sliderData:
          pageBuilder.AddSliderOption(name,
              loadValue: () => sliderData.CurrentValue,
              saveValue: (value) => Log.Trace($"TODO: Save value '{value}' for slider {name}."));
          return;
        default:
          // TODO: Implement TextField widget and use it to display TextFieldData.
          Log.Error($"Unsupported widget type for config field {name}. Not adding it to the menu.");
          break;
      }
    }
  }
}
