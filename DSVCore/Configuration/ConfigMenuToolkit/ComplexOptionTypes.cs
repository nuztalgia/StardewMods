using System;
using System.Reflection;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;
using Nuztalgia.StardewMods.Common.UI;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class GMCMIntegrationExtensions {

  internal static Integration AddDefaultSpacing(this Integration configMenu) {
    Spacing.DefaultVertical.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddStaticParagraph(this Integration configMenu, Func<string> getText) {
    return configMenu.AddStaticParagraph(getText());
  }

  internal static Integration AddStaticParagraph(this Integration configMenu, string text) {
    StaticText.CreateParagraph(text).AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddStaticHeader(this Integration configMenu, Func<string> getText) {
    return configMenu.AddStaticHeader(getText());
  }

  internal static Integration AddStaticHeader(this Integration configMenu, string text) {
    new Header(text).AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddCompatSectionHeader(
      this Integration configMenu, BaseCompatSection section) {

    if ((section as BaseSyncedCompatSection)?.GetModManifest() is not IManifest modManifest) {
      return configMenu.AddStaticHeader(section.GetDisplayName());
    }

    Header.WithButton header = new(
        headerText: modManifest.Name,
        buttonText: I18n.Compat_Synced_OpenMenu(),
        buttonAction: () => configMenu.Api.OpenModMenu(modManifest));

    header.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddCheckbox(
      this Integration configMenu,
      BaseMenuSection section,
      PropertyInfo property,
      string optionName,
      string tooltip,
      string fieldId,
      Action<string, object> onValueChanged) {

    Checkbox checkbox = new(
        name: optionName,
        tooltip: tooltip,
        loadValue: () => (bool) property.GetValue(section)!,
        saveValue: (bool value) => property.SetValue(section, value),
        onValueChanged: (newValue) => onValueChanged(fieldId, newValue));

    checkbox.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddSlider(
      this Integration configMenu,
      BaseMenuSection section,
      PropertyInfo property,
      string optionName,
      string tooltip,
      string fieldId,
      Action<string, object> onValueChanged) {

    Slider slider = new(
        name: optionName,
        tooltip: tooltip,
        loadValue: () => (int) property.GetValue(section)!,
        saveValue: (int value) => property.SetValue(section, value),
        staticMinValue: section.GetMinValue(property),
        getDynamicMaxValue: () => section.GetMaxValue(property),
        onValueChanged: (newValue) => onValueChanged(fieldId, newValue));

    slider.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddCharacterPreviews(
      this Integration configMenu,
      CharacterConfigState characterState,
      string optionName,
      string tooltip) {

    CharacterPreviewImage characterPreview = new(characterState);

    configMenu.AddComplexOption(
        optionName: optionName,
        getHeight: characterPreview.GetHeight,
        drawAction: characterPreview.Draw,
        resetAction: characterState.ResetState,
        saveAction: characterState.SaveState,
        tooltip: tooltip);

    return configMenu;
  }

  internal static Integration AddCharacterThumbnails(
      this Integration configMenu,
      BaseSyncedCompatSection syncedSection,
      CharacterConfigState.LoadImage loadGameImage) {

    CharacterThumbnails characterThumbnails = new(
        syncedSection.GetSyncedItems,
        CharacterConfigState.GetPortraitData,
        loadGameImage,
        I18n.Compat_Synced_None().Format(syncedSection.GetDisplayName()),
        I18n.Compat_Synced_Error().Format(syncedSection.GetDisplayName()));

    configMenu.AddComplexOption(
        optionName: string.Empty,
        getHeight: characterThumbnails.GetHeight,
        drawAction: characterThumbnails.Draw,
        resetAction: characterThumbnails.Update);

    return configMenu;
  }
}
