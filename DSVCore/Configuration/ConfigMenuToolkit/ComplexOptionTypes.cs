using System;
using System.Reflection;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class GMCMIntegrationExtensions {

  internal static Integration AddDynamicSlider(
      this Integration configMenu,
      BaseMenuSection section,
      PropertyInfo property,
      string optionName,
      string tooltip,
      string fieldId,
      Action<string, object> onValueChange) {

    DynamicSlider dynamicSlider = new(
        name: optionName,
        tooltip: tooltip,
        getValue: () => (int) property.GetValue(section)!,
        saveValue: (int value) => property.SetValue(section, value),
        staticMinValue: section.GetMinValue(property),
        getDynamicMaxValue: () => section.GetMaxValue(property),
        onValueChange: (newValue) => onValueChange(fieldId, newValue));

    dynamicSlider.AddToConfigMenu(configMenu.Api, configMenu.Manifest);
    return configMenu;
  }

  internal static Integration AddCharacterPreview(
      this Integration configMenu,
      BaseCharacterSection character,
      string optionName,
      CharacterConfigState.LoadImage loadGameImage,
      CharacterConfigState.LoadImage loadModImage) {

    var characterState = CharacterConfigState.Create(
        character.Name,
        loadGameImage,
        loadModImage,
        character.GetModImagePaths,
        character.GetGameImagePaths,
        character.GetPortraitRectsDelegate(),
        character.GetSpriteRectsDelegate());
    CharacterPreviewImage characterPreview = new(characterState);

    configMenu.AddComplexOption(
        optionName: optionName,
        getHeight: characterPreview.GetHeight,
        drawAction: characterPreview.Draw,
        resetAction: characterState.ResetState,
        saveAction: characterState.SaveState,
        tooltip: character.GetPreviewTooltip());

    character.GetOptions().ForEach(item => CharacterConfigState.Update(item.UniqueId, item.Value));
    characterState.SaveState();

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
