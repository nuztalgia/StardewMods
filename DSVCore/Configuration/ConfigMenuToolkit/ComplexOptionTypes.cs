using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class GMCMIntegrationExtensions {

  private static readonly Func<string, string> FormatEnumValue =
      value => I18nHelper.GetStringByKeyName($"Value_{value}") ?? value;

  internal static GenericModConfigMenuIntegration AddSectionOptions(
      this GenericModConfigMenuIntegration configMenu,
      BaseMenuSection section) {

    foreach (BaseMenuSection.OptionItem item in section.GetOptions()) {
      switch (item.Value) {
        case bool:
          configMenu.AddCheckbox(section, item.Property, item.Name, item.Tooltip, item.UniqueId);
          break;
        case int:
          configMenu.AddSlider(
              section, item.Property, item.Name, item.Tooltip, item.UniqueId,
              staticMinValue: section.GetMinValue(item.Property),
              getDynamicMaxValue: () => section.GetMaxValue(item.Property));
          break;
        case Enum:
          // TODO: Finish implementing the Dropdown widget and change this case to use it.
          configMenu.AddEnumOption(
              section, item.Property, item.Name, FormatEnumValue, item.Tooltip, item.UniqueId);
          break;
        default:
          Log.Error($"Unexpected type '{item.Value?.GetType()}' for option '{item.UniqueId}'.");
          break;
      }
    }
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
