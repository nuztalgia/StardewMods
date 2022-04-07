using GenericModConfigMenu;
using Nuztalgia.StardewMods.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal class GenericModConfigMenuIntegration : BaseIntegration<IGenericModConfigMenuApi> {

  internal GenericModConfigMenuIntegration() :
      base(Globals.ModRegistry, "spacechase0.GenericModConfigMenu") { }

  internal void SetUpConfigMenu(Action<ModConfig> writeConfig) {
    if (this.Api is null) {
      Log.Warn("Could not retrieve the Generic Mod Config Menu API. " +
               "Some functionality of this mod will be disabled.");
      return;
    }

    this.Api.Register(
        mod: Globals.Manifest,
        reset: () => Globals.UpdateActiveConfig(this, new ModConfig()),
        save: () => writeConfig(Globals.Config)
    );

    List<BaseContentPackOptions> availableOptions = new();
    List<BaseContentPackOptions> unavailableOptions = new();

    foreach (PropertyInfo? propertyInfo in Globals.Config.GetType().GetProperties()) {
      if (propertyInfo?.GetValue(Globals.Config) is BaseContentPackOptions options) {
        (options.IsContentPackLoaded() ? availableOptions : unavailableOptions).Add(options);
      }
    }

    this.SetUpMainPage(availableOptions, unavailableOptions);

    foreach (BaseContentPackOptions options in availableOptions) {
      this.SetUpOptionsPage(options);
    }
  }

  private void SetUpMainPage(IEnumerable<BaseContentPackOptions> availableOptions,
                             IEnumerable<BaseContentPackOptions> unavailableOptions) {
    this.AddSectionTitle(I18n.Settings_Intro_Title)
        .AddParagraph(I18n.Settings_Intro_Description)
        .AddSectionTitle(I18n.Settings_InstalledPacks_Title)
        .AddParagraph(availableOptions.Any()
                      ? I18n.Settings_InstalledPacks_Description
                      : I18n.Settings_InstalledPacks_None);

    foreach (BaseContentPackOptions options in availableOptions) {
      Log.Trace($"'{options.GetDisplayName()}' pack is installed. Adding config menu.");
      this.AddPageLink(options.Name, $" > {options.GetDisplayName()}");
    }

    this.AddSpacing()
        .AddSectionTitle(I18n.Settings_OtherPacks_Title)
        .AddParagraph(unavailableOptions.Any()
                      ? I18n.Settings_OtherPacks_Description
                      : I18n.Settings_OtherPacks_None);

    foreach (BaseContentPackOptions options in unavailableOptions) {
      Log.Trace($"'{options.GetDisplayName()}' pack is *NOT* installed.");
      this.AddSectionTitle(() => $" * {options.GetDisplayName()}");
    }
  }

  private void SetUpOptionsPage(BaseContentPackOptions options) {
    this.AddPage(options.Name, options.GetDisplayName());
    foreach (PropertyInfo? propertyInfo in options.GetType().GetProperties()) {
      if (propertyInfo?.GetValue(options) is BaseOptions character) {
        this.AddSectionTitle(() => character.Name)
            .AddCharacterOptions(character)
            .AddSpacing();
      } else {
        Type? type = propertyInfo?.PropertyType;
        Log.Warn($"Unexpected type '{type}' for property '{propertyInfo?.Name}'.");
      }
    }
  }

  private GenericModConfigMenuIntegration AddCharacterOptions(BaseOptions character) {
    Log.Verbose($"Current config options for {character.Name}: {character}");
    foreach (PropertyInfo? propertyInfo in character.GetType().GetProperties()) {
      string propertyName = Globals.GetI18nString($"Settings_{propertyInfo.Name}_Name");
      switch (propertyInfo.GetValue(character)) {
        case Enum:
          this.AddEnumOption(propertyName, propertyInfo, character);
          break;
        case bool:
          this.AddBoolOption(propertyName, propertyInfo, character);
          break;
        case int:
          // TODO: Set maximum value properly.
          this.AddIntOption(propertyName, propertyInfo, character, max: 5);
          break;
        case string:
          // TODO: Figure out whether string options will actually end up being used.
          break;
        default:
          Type type = propertyInfo.PropertyType;
          Log.Warn($"Unexpected type '{type}' for property '{propertyInfo.Name}'.");
          break;
      }
    }
    return this;
  }

  private GenericModConfigMenuIntegration AddSpacing() {
    return this.AddSectionTitle(() => "");
  }

  private GenericModConfigMenuIntegration AddSectionTitle(Func<string> text) {
    this.Api!.AddSectionTitle(Globals.Manifest, text);
    return this;
  }

  private GenericModConfigMenuIntegration AddParagraph(Func<string> text) {
    this.Api!.AddParagraph(Globals.Manifest, text);
    return this;
  }

  private GenericModConfigMenuIntegration AddPage(string pageId, string pageTitle) {
    this.Api!.AddPage(Globals.Manifest, pageId, () => pageTitle);
    return this;
  }

  private GenericModConfigMenuIntegration AddPageLink(string pageId, string text) {
    this.Api!.AddPageLink(Globals.Manifest, pageId, () => text);
    return this;
  }

  private GenericModConfigMenuIntegration AddBoolOption(
      string name, PropertyInfo propertyInfo, BaseOptions optionsContainer) {
    this.Api!.AddBoolOption(
        mod: Globals.Manifest,
        name: () => name,
        getValue: () => ((bool?) propertyInfo.GetValue(optionsContainer)) ?? false,
        setValue: value => propertyInfo.SetValue(optionsContainer, value)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddIntOption(
      string name, PropertyInfo propertyInfo, BaseOptions optionsContainer, int? max = null) {
    this.Api!.AddNumberOption(
        mod: Globals.Manifest,
        name: () => name,
        min: 1,
        max: max,
        getValue: () => ((int?) propertyInfo.GetValue(optionsContainer)) ?? 1,
        setValue: value => propertyInfo.SetValue(optionsContainer, value)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddEnumOption(
      string name, PropertyInfo propertyInfo, BaseOptions optionsContainer) {
    this.Api!.AddTextOption(
        mod: Globals.Manifest,
        name: () => name,
        allowedValues: Enum.GetNames(propertyInfo.PropertyType),
        getValue: () => propertyInfo.GetValue(optionsContainer)?.ToString() ?? "",
        setValue: value => propertyInfo.SetValue(optionsContainer,
                                                 Enum.Parse(propertyInfo.PropertyType, value))
    );
    return this;
  }
}
