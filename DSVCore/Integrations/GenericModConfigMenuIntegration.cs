using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;

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
      this.SetUpContentPackPage(contentPack);
    }
  }

  private void OnFieldChanged(string fieldId, object? newValue) {
    Log.Verbose($"Field '{fieldId}' was changed to: '{newValue}'.");
    if (newValue is string valueDisplayName) {
      newValue = ValueNameLookup.GetValueName(valueDisplayName);
    }
    ImagePreviews.SetFieldValue(fieldId, newValue);
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
    IEnumerable<BaseMenuSection> compatSections = coreAndCompat.GetAvailableCompatSections();

    this.AddPage(coreAndCompat.Name, coreAndCompat.GetDisplayName())
        .AddSectionTitle(coreOptions.GetDisplayName())
        .AddParagraph(I18n.Core_Section_Description)
        .AddSectionOptions(coreOptions)
        .AddSpacing();

    foreach (BaseMenuSection section in compatSections) {
      this.AddSectionTitle(section.GetDisplayName())
          .AddSectionOptions(section)
          .AddSpacing();
    }

    // Show the placeholder if there are no compat mods or if the only one is Flower Queen's Crown.
    if (!compatSections.Any() || ((compatSections.Count() == 1)
                                  && (compatSections.First().Name == "FlowerQueensCrown"))) {
      this.AddSectionTitle(I18n.Compat_Placeholder_Title)
          .AddParagraph(I18n.Compat_Placeholder_Description);
    }
  }

  private void SetUpContentPackPage(BaseContentPackPage contentPack) {
    this.AddPage(contentPack.Name, contentPack.GetDisplayName());

    foreach (BaseCharacterSection character in contentPack.GetAllSections()) {
      ImagePreviews.InitializeCharacter(character.Name,
          Globals.ContentHelper.Load<Texture2D>, contentPack.GetModContentHelper().Load<Texture2D>, 
          character.GetPreviewImagePath, character.GetPortraitRect(), character.GetSpriteRect());

      this.AddSectionTitle(character.GetDisplayName())
          .AddSectionOptions(character)
          .AddComplexOption(
              name: " =  " + I18n.Option_Preview(),
              tooltip: string.Format(I18n.Tooltip_Preview(), character.Name),
              height: ImagePreviews.GetHeight(character.Name),
              drawAction: (sb, position) => ImagePreviews.Draw(character.Name, sb, position));
    }
  }

  private GenericModConfigMenuIntegration AddSectionOptions(BaseMenuSection section) {
    foreach (BaseMenuSection.OptionItem item in section.GetOptions()) {
      string fieldId = item.UniqueId;
      string displayName = " >  " + item.Name;
      ImagePreviews.SetFieldValue(fieldId, item.Value);

      switch (item.Value) {
        case Enum:
          this.AddEnumOption(section, fieldId, displayName, item.Tooltip, item.Property);
          break;
        case bool:
          this.AddBoolOption(section, fieldId, displayName, item.Tooltip, item.Property);
          break;
        case int:
          this.AddIntOption(section, fieldId, displayName, item.Tooltip, item.Property,
                            section.GetMinValue(item.Property), section.GetMaxValue(item.Property));
          break;
        default:
          Log.Error($"Unexpected type '{item.Value?.GetType()}' for option '{fieldId}'.");
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
      BaseMenuSection section, string fieldId, string name, string tooltip, PropertyInfo property) {
    this.Api!.AddTextOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => property.GetValueDisplayName(section),
        setValue: valueDisplayName => property.SetValueByDisplayName(section, valueDisplayName),
        allowedValues: ValueNameLookup.GetAllValueDisplayNames(property).ToArray()
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddBoolOption(
      BaseMenuSection section, string fieldId, string name, string tooltip, PropertyInfo property) {
    this.Api!.AddBoolOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => ((bool?) property.GetValue(section)) ?? false,
        setValue: value => property.SetValue(section, value)
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddIntOption(
      BaseMenuSection section, string fieldId, string name, string tooltip, 
      PropertyInfo property, int? min = null, int? max = null) {
    this.Api!.AddNumberOption(
        mod: Globals.Manifest,
        fieldId: fieldId,
        name: () => name,
        tooltip: () => tooltip,
        getValue: () => ((int?) property.GetValue(section)) ?? 0,
        setValue: value => property.SetValue(section, value),
        min: min,
        max: max
    );
    return this;
  }

  private GenericModConfigMenuIntegration AddComplexOption(
      string name, string tooltip, int height, Action<SpriteBatch, Vector2> drawAction) {
    this.Api!.AddComplexOption(
        mod: Globals.Manifest,
        name: () => name,
        tooltip: () => tooltip,
        height: () => height,
        draw: drawAction
    );
    return this;
  }
}
