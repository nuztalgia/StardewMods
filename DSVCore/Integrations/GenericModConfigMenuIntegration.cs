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
      this.AddPageLink(options.GetPageId(), $" > {options.GetDisplayName()}");
    }

    this.AddSpacing()
        .AddSectionTitle(I18n.Settings_OtherPacks_Title)
        .AddParagraph(unavailableOptions.Any()
                      ? I18n.Settings_OtherPacks_Description
                      : I18n.Settings_OtherPacks_None);

    foreach (BaseContentPackOptions options in unavailableOptions) {
      this.AddSectionTitle(() => $" * {options.GetDisplayName()}");
    }
  }

  private void SetUpOptionsPage(BaseContentPackOptions options) {
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

  /*
  this.API.AddSectionTitle(this.Manifest, () => "Abigail");

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.AbigailVariant),
    allowedValues: Enum.GetNames(typeof(Variants.Abigail)),
    getValue: () => this.Config.AbigailVariant.ToString(),
    setValue: value => this.Config.AbigailVariant = Enum.Parse<Variants.Abigail>(value)
  );

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.AbigailLightweightConfig),
    allowedValues: Enum.GetNames(typeof(LightweightConfig)),
    getValue: () => this.Config.AbigailLightweightConfig.ToString(),
    setValue: value => this.Config.AbigailLightweightConfig = Enum.Parse<LightweightConfig>(value)
  );

  this.API.AddNumberOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.AbigailWeddingOutfit),
    min: 1,
    max: 5,
    getValue: () => this.Config.AbigailWeddingOutfit,
    setValue: value => this.Config.AbigailWeddingOutfit = value
  );

  this.API.AddBoolOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.AbigailMermaidPendant),
    getValue: () => this.Config.AbigailMermaidPendant,
    setValue: value => this.Config.AbigailMermaidPendant = value
  );

  this.API.AddSectionTitle(this.Manifest, () => "Caroline");

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.CarolineVariant),
    allowedValues: Enum.GetNames(typeof(Variants.Basic)),
    getValue: () => this.Config.CarolineVariant.ToString(),
    setValue: value => this.Config.CarolineVariant = Enum.Parse<Variants.Basic>(value)
  );

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.CarolineLightweightConfig),
    allowedValues: Enum.GetNames(typeof(LightweightConfig)),
    getValue: () => this.Config.CarolineLightweightConfig.ToString(),
    setValue: value => this.Config.CarolineLightweightConfig = Enum.Parse<LightweightConfig>(value)
  );

  this.API.AddSectionTitle(this.Manifest, () => "Pierre");

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.PierreVariant),
    allowedValues: Enum.GetNames(typeof(Variants.Basic)),
    getValue: () => this.Config.PierreVariant.ToString(),
    setValue: value => this.Config.PierreVariant = Enum.Parse<Variants.Basic>(value)
  );

  this.API.AddTextOption(
    mod: this.Manifest,
    name: () => nameof(this.Config.PierreLightweightConfig),
    allowedValues: Enum.GetNames(typeof(LightweightConfig)),
    getValue: () => this.Config.PierreLightweightConfig.ToString(),
    setValue: value => this.Config.PierreLightweightConfig = Enum.Parse<LightweightConfig>(value)
  );
  */
}
