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
    this.Api.OnFieldChanged(Globals.Manifest, this.OnFieldChanged);

    List<BaseContentPackOptions> installedPacks = new();
    List<BaseContentPackOptions> otherPacks = new();

    foreach (PropertyInfo property in Globals.Config.GetType().GetProperties()) {
      if (property.GetValue(Globals.Config) is BaseContentPackOptions contentPack) {
        (contentPack.IsContentPackLoaded() ? installedPacks : otherPacks).Add(contentPack);
      }
    }

    this.SetUpMainPage(installedPacks, otherPacks);

    foreach (BaseContentPackOptions contentPack in installedPacks) {
      this.SetUpContentPackPage(contentPack);
    }
  }

  private void OnFieldChanged(string fieldId, object newValue) {
    Log.Trace($"Field '{fieldId}' was changed to: '{newValue}'.");
    // TODO: Handle field changes somehow.
  }

  private void SetUpMainPage(IEnumerable<BaseContentPackOptions> installedPacks,
                             IEnumerable<BaseContentPackOptions> otherPacks) {
    this.AddSectionTitle(I18n.Settings_Intro_Title)
        .AddParagraph(I18n.Settings_Intro_Description)
        .AddSectionTitle(I18n.Settings_InstalledPacks_Title)
        .AddParagraph(installedPacks.Any()
                      ? I18n.Settings_InstalledPacks_Description
                      : I18n.Settings_InstalledPacks_None);

    foreach (BaseContentPackOptions contentPack in installedPacks) {
      Log.Trace($"'{contentPack.GetDisplayName()}' pack is installed. Adding config menu.");
      this.AddPageLink(contentPack.Name, $" > {contentPack.GetDisplayName()}");
    }

    this.AddSpacing()
        .AddSectionTitle(I18n.Settings_OtherPacks_Title)
        .AddParagraph(otherPacks.Any()
                      ? I18n.Settings_OtherPacks_Description
                      : I18n.Settings_OtherPacks_None);

    foreach (BaseContentPackOptions contentPack in otherPacks) {
      Log.Trace($"'{contentPack.GetDisplayName()}' pack is *NOT* installed.");
      this.AddSectionTitle(() => $" * {contentPack.GetDisplayName()}");
    }
  }

  private void SetUpContentPackPage(BaseContentPackOptions contentPack) {
    this.AddPage(contentPack.Name, contentPack.GetDisplayName());
    foreach (PropertyInfo property in contentPack.GetType().GetProperties()) {
      if (property.GetValue(contentPack) is BaseOptions character) {
        this.AddSectionTitle(() => character.Name)
            .AddCharacterOptions(character)
            .AddSpacing();
      } else {
        Log.Error($"Invalid type '{property.PropertyType}' for property '{property.Name}'.");
      }
    }
  }

  private GenericModConfigMenuIntegration AddCharacterOptions(BaseOptions character) {
    foreach (PropertyInfo property in character.GetType().GetProperties()) {
      // TODO: Support other string namespaces instead of hardcoding it like this.
      string optionName = Globals.GetI18nString($"Settings_{property.Name}_Name");
      string fieldId = $"{character.Name}_{property.Name}";
      object? propertyValue = property.GetValue(character);
      Log.Verbose($"Option: {fieldId,-28}|  Value: {propertyValue}");

      switch (propertyValue) {
        case Enum:
          this.AddEnumOption(fieldId, optionName, property, character);
          break;
        case bool:
          this.AddBoolOption(fieldId, optionName, property, character);
          break;
        case int:
          // TODO: Set minimum and maximum values properly.
          this.AddIntOption(fieldId, optionName, property, character, min: 1, max: 5);
          break;
        case string:
          // TODO: Figure out whether string options will actually end up being used.
          // For now, just fall through and log an error.
        default:
          Log.Error($"Unexpected type '{property.PropertyType}' for property '{property.Name}'.");
          break;
      }
    }
    return this;
  }

  private GenericModConfigMenuIntegration AddSpacing() {
    return this.AddSectionTitle(() => string.Empty);
  }

  private GenericModConfigMenuIntegration AddParagraph(Func<string> text) {
    this.Api!.AddParagraph(Globals.Manifest, text);
    return this;
  }

  private GenericModConfigMenuIntegration AddSectionTitle(Func<string> text) {
    this.Api!.AddSectionTitle(Globals.Manifest, text);
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

  private GenericModConfigMenuIntegration AddEnumOption(
      string fieldId, string name, PropertyInfo property, object container) {
    this.Api!.AddTextOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        getValue: () => property.GetValue(container)?.ToString() ?? string.Empty,
        setValue: value => property.SetValue(container, Enum.Parse(property.PropertyType, value)),
        allowedValues: Enum.GetNames(property.PropertyType)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddBoolOption(
      string fieldId, string name, PropertyInfo property, object container) {
    this.Api!.AddBoolOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        getValue: () => ((bool?) property.GetValue(container)) ?? false,
        setValue: value => property.SetValue(container, value)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddIntOption(
      string fieldId, string name, PropertyInfo property, object container,
      int? min = null, int? max = null) {
    this.Api!.AddNumberOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        getValue: () => ((int?) property.GetValue(container)) ?? 0,
        setValue: value => property.SetValue(container, value),
        min: min,
        max: max
    );
    return this;
  }
}
