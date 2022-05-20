using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Nuztalgia.StardewMods.Common.UI;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class GMCMIntegrationExtensions {

  internal static GenericModConfigMenuIntegration AddDefaultSpacing(
      this GenericModConfigMenuIntegration configMenu) {
    Spacing.DefaultVertical.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static GenericModConfigMenuIntegration AddStaticParagraph(
      this GenericModConfigMenuIntegration configMenu, Func<string> getText) {
    return configMenu.AddStaticParagraph(getText());
  }

  internal static GenericModConfigMenuIntegration AddStaticParagraph(
      this GenericModConfigMenuIntegration configMenu, string text) {
    StaticText.CreateParagraph(text).AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static GenericModConfigMenuIntegration AddStaticHeader(
      this GenericModConfigMenuIntegration configMenu, Func<string> getText) {
    return configMenu.AddStaticHeader(getText());
  }

  internal static GenericModConfigMenuIntegration AddStaticHeader(
      this GenericModConfigMenuIntegration configMenu, string text) {
    new Header(text).AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static GenericModConfigMenuIntegration AddCompatSectionHeader(
      this GenericModConfigMenuIntegration configMenu, BaseCompatSection section) {

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

  internal static GenericModConfigMenuIntegration AddCheckbox(
      this GenericModConfigMenuIntegration configMenu,
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

  internal static GenericModConfigMenuIntegration AddSlider(
      this GenericModConfigMenuIntegration configMenu,
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

  internal static GenericModConfigMenuIntegration AddCharacterPreviews(
      this GenericModConfigMenuIntegration configMenu,
      CharacterConfigState charState,
      string optionName,
      string tooltip) {

    ImageGroup imageGroup = ImageGroup.CreateHorizontal(optionName, tooltip,
        charState.ResetState, charState.SaveState, innerPadding: 32, bottomAlign: true);

    AddCharacterPreview(imageGroup, charState, CharacterConfigState.PortraitsDirectory, scale: 3);
    AddCharacterPreview(imageGroup, charState, CharacterConfigState.SpritesDirectory, scale: 5);

    Spacing.CreateVertical(height: 16).AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    imageGroup.AddToConfigMenu(configMenu.Api, configMenu.Manifest);

    return configMenu;
  }

  internal static GenericModConfigMenuIntegration AddCharacterThumbnails(
      this GenericModConfigMenuIntegration configMenu,
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

  private static void AddCharacterPreview(
      ImageGroup imageGroup, CharacterConfigState charState, string imageDirectory, int scale) {

    // This assumes all portrait assets are the same size & all sprite assets are the same size.
    Rectangle? sizingRect = charState.GetSourceRects(imageDirectory)?[0][0];

    for (int i = 0; i < charState.GetNumberOfImages(imageDirectory); ++i) {
      int index = i; // Closure. The lines below won't work if "i" is used in place of "index".

      imageGroup.AddImage(
          getSourceImages: () => GetArray(charState.GetSourceImages(imageDirectory), index),
          getSourceRects: () => GetArray(charState.GetSourceRects(imageDirectory), index),
          fixedWidth: sizingRect?.Width,
          fixedHeight: sizingRect?.Height,
          scale: scale);
    }

    static TArray[] GetArray<TArray>(TArray[][]? source, int index) {
      return ((source == null) || (source.Length <= index)) ? Array.Empty<TArray>() : source[index];
    }
  }
}
