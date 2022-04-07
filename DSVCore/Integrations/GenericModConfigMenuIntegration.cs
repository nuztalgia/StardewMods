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
      this.AddPageLink(options.GetPageId(), $" > {options.GetDisplayName()}");
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
    Log.Verbose($"Current config options for {options.GetDisplayName()}: {options}");
    this.AddPage(options.GetPageId(), options.GetDisplayName());
    // TODO: Actually add options to each page.
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
}
