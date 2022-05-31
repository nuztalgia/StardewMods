namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal static class ConfigMenuBuilder {

  internal record ModMenuData(
      IManifest Manifest,
      IEnumerable<ConfigFieldData> ConfigFields,
      MenuSpec MenuSpec);

  internal static void PopulateConfigMenu(
      GenericModConfigMenuIntegration configMenu,
      IDictionary<string, ModMenuData> modMenuData) {

    foreach ((string modId, ModMenuData menuData) in modMenuData) {
      // TODO: Support menus with multiple pages.
      IConfigPageBuilder pageBuilder = configMenu.CreateSinglePageMenuBuilder(menuData.Manifest);
      pageBuilder.AddStaticHeader(menuData.Manifest.Name);

      // TODO: Incorporate additional widget information from MenuSpec.
      foreach (ConfigFieldData configField in menuData.ConfigFields) {
        AddConfigFieldWidgetToPage(pageBuilder, configField);
      }

      pageBuilder.EndPage().PublishMenu();
    }
  }

  internal static void AddConfigFieldWidgetToPage(
      IConfigPageBuilder pageBuilder, ConfigFieldData configField) {
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
