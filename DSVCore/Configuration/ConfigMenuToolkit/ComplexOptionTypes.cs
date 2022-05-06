using System;
using System.Reflection;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class IntOptionConfigSource : DynamicSlider.IDataSource {
  public Func<int> GetMinValue { get; init; }
  public Func<int> GetMaxValue { get; init; }
  public int Value { get; set; }

  private readonly string? FieldId;
  private readonly Action<string, object> OnFieldChanged;

  internal IntOptionConfigSource(
      string? fieldId,
      int initialValue,
      Func<int> getMinValue,
      Func<int> getMaxValue,
      Action<string, object> onFieldChanged) {

    this.FieldId = fieldId;
    this.Value = Math.Clamp(initialValue, getMinValue(), getMaxValue());
    this.GetMinValue = getMinValue;
    this.GetMaxValue = getMaxValue;
    this.OnFieldChanged = onFieldChanged;
  }

  public void OnValueChanged(int newValue) {
    if (this.FieldId is not null) {
      this.OnFieldChanged(this.FieldId, newValue);
    } else {
      Log.Trace($"Slider value changed to {newValue}, but no field ID was specified to update.");
    }
  }
}

internal static class GMCMIntegrationExtensions {

  internal static Integration AddDynamicSlider(
      this Integration configMenu,
      BaseMenuSection section,
      PropertyInfo property,
      string optionName,
      int initialValue,
      Action<string, object> onFieldChanged,
      string? tooltip = null,
      string? fieldId = null) {

    IntOptionConfigSource dataSource = new(
        fieldId,
        initialValue,
        () => section.GetMinValue(property),
        () => section.GetMaxValue(property),
        onFieldChanged);
    DynamicSlider dynamicSlider = new(dataSource);

    configMenu.AddComplexOption(
        optionName: optionName,
        getHeight: () => DynamicSlider.TotalHeight,
        drawAction: dynamicSlider.Draw,
        resetAction: () => {
          (int min, int max) = (section.GetMinValue(property), section.GetMaxValue(property));
          int propertyValue = ((int?) property.GetValue(section)) ?? min;
          dataSource.Value = Math.Clamp(propertyValue, min, max);
        },
        saveAction: () => property.SetValue(section, dataSource.Value),
        tooltip: tooltip,
        fieldId: fieldId);

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
