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

    CoreAndCompatPage coreAndCompat = Globals.Config.CoreAndCompat;
    List<BaseContentPackPage> installedPacks = new();
    List<BaseContentPackPage> otherPacks = new();

    foreach (PropertyInfo property in Globals.Config.GetType().GetProperties()) {
      if (property.GetValue(Globals.Config) is BaseContentPackPage contentPack) {
        (contentPack.IsAvailable() ? installedPacks : otherPacks).Add(contentPack);
      }
    }

    this.SetUpMainPage(coreAndCompat, installedPacks, otherPacks);
    this.SetUpCoreAndCompatPage(coreAndCompat);

    foreach (BaseContentPackPage contentPack in installedPacks) {
      this.AddPage(contentPack.Name, contentPack.GetDisplayName())
          .SetUpAvailableSections(contentPack.GetAllSections());
    }
  }

  private void OnFieldChanged(string fieldId, object newValue) {
    Log.Verbose($"Field '{fieldId}' was changed to: '{newValue}'.");
    // TODO: Handle field changes somehow.
  }

  private void SetUpMainPage(CoreAndCompatPage coreAndCompat,
                             IEnumerable<BaseContentPackPage> installedPacks,
                             IEnumerable<BaseContentPackPage> otherPacks) {
    this.AddSectionTitle(I18n.Main_Intro_Title)
        .AddParagraph(I18n.Main_Intro_Description)
        .AddPageLink(coreAndCompat.Name, $" > {coreAndCompat.GetDisplayName()}")
        .AddSpacing()
        .AddSectionTitle(I18n.Main_InstalledPacks_Title)
        .AddParagraph(installedPacks.Any()
                      ? I18n.Main_InstalledPacks_Description
                      : I18n.Main_InstalledPacks_None);

    foreach (BaseContentPackPage contentPack in installedPacks) {
      Log.Trace($"'{contentPack.GetDisplayName()}' pack is installed. Adding config menu.");
      this.AddPageLink(contentPack.Name, $" > {contentPack.GetDisplayName()}");
    }

    this.AddSpacing()
        .AddSectionTitle(I18n.Main_OtherPacks_Title)
        .AddParagraph(otherPacks.Any()
                      ? I18n.Main_OtherPacks_Description
                      : I18n.Main_OtherPacks_None);

    foreach (BaseContentPackPage contentPack in otherPacks) {
      Log.Trace($"'{contentPack.GetDisplayName()}' pack is *NOT* installed.");
      this.AddSectionTitle($" * {contentPack.GetDisplayName()}");
    }
  }

  private void SetUpCoreAndCompatPage(CoreAndCompatPage coreAndCompat) {
    BaseMenuSection coreOptions = coreAndCompat.CoreOptions;
    IEnumerable<BaseMenuSection> compatSections = coreAndCompat.GetCompatSections();

    this.AddPage(coreAndCompat.Name, coreAndCompat.GetDisplayName())
        .AddSectionTitle(coreOptions.GetDisplayName())
        .AddParagraph(I18n.Core_Section_Description)
        .AddSectionOptions(coreOptions)
        .AddSpacing();

    if (compatSections.Any()) {
      this.SetUpAvailableSections(compatSections);
    } else {
      this.AddSectionTitle(I18n.Compat_Placeholder_Title)
          .AddParagraph(I18n.Compat_Placeholder_Description);
    }
  }

  private void SetUpAvailableSections(IEnumerable<BaseMenuSection> sections) {
    foreach (BaseMenuSection section in sections) {
      if (section.IsAvailable()) {
        this.AddSectionTitle(section.GetDisplayName())
            .AddSectionOptions(section)
            .AddSpacing();
      }
    }
  }

  private GenericModConfigMenuIntegration AddSectionOptions(BaseMenuSection section) {
    foreach (BaseMenuSection.OptionItem item in section.GetOptions()) {
      Log.Verbose($"Option: {item.UniqueId, -25}|  Value: {item.Value}");
      string displayName = " >  " + item.Name;
      switch (item.Value) {
        case Enum:
          this.AddEnumOption(section, item.UniqueId, displayName, item.Tooltip, item.Property);
          break;
        case bool:
          this.AddBoolOption(section, item.UniqueId, displayName, item.Tooltip, item.Property);
          break;
        case int:
          this.AddIntOption(section, item.UniqueId, displayName, item.Tooltip, item.Property,
                            section.GetMinValue(item.Property), section.GetMaxValue(item.Property));
          break;
        default:
          Log.Error($"Unexpected type '{item.Value?.GetType()}' for option '{item.UniqueId}'.");
          break;
      }
    }
    return this;
  }

  private GenericModConfigMenuIntegration AddSpacing() {
    return this.AddSectionTitle(string.Empty);
  }

  private GenericModConfigMenuIntegration AddSectionTitle(string text) {
    return this.AddSectionTitle(() => text);
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

  private GenericModConfigMenuIntegration AddEnumOption(
      BaseMenuSection container, string fieldId, string name, string tooltip, PropertyInfo property) {
    this.Api!.AddTextOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => property.GetValue(container)?.ToString() ?? string.Empty,
        setValue: value => property.SetValue(container, Enum.Parse(property.PropertyType, value)),
        allowedValues: Enum.GetNames(property.PropertyType)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddBoolOption(
      BaseMenuSection container, string fieldId, string name, string tooltip, PropertyInfo property) {
    this.Api!.AddBoolOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => ((bool?) property.GetValue(container)) ?? false,
        setValue: value => property.SetValue(container, value)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddIntOption(
      BaseMenuSection container, string fieldId, string name, string tooltip, 
      PropertyInfo property, int? min = null, int? max = null) {
    this.Api!.AddNumberOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => ((int?) property.GetValue(container)) ?? 0,
        setValue: value => property.SetValue(container, value),
        min: min,
        max: max
    );
    return this;
  }
}
