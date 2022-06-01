namespace Nuztalgia.StardewMods.DynamicMenuOptions;

public sealed class MenuPageItem {

  public sealed class RawData {
    // Exactly ONE of the following fields MUST be set to determine how this item is handled.
    public string? Type;
    public string? FromTemplate;
    public string? FromConfig;
    public string? FromFile;
  }

  internal static IEnumerable<MenuPageItem> ResolveData(string modId, IEnumerable<RawData> data) {
    foreach (RawData dataItem in data) {
      foreach (MenuPageItem pageItem in ResolveDataItem(modId, dataItem)) {
        yield return pageItem;
      }
    }
  }

  private static IEnumerable<MenuPageItem> ResolveDataItem(string modId, RawData dataItem) {
    if (dataItem.FromFile is string fileName) {
      List<RawData>? dataFromFile =
          ModRegistryUtils.LoadListDataFromContentPack<RawData>(modId, fileName);

      if (dataFromFile?.Any() is true) {
        foreach (MenuPageItem pageItem in ResolveData(modId, dataFromFile)) {
          yield return pageItem;
        }
      } else {
        Log.Error($"Could not parse menu information from file '{fileName}' in mod '{modId}'.");
      }
    } else if (ResolveSingleItem(modId, dataItem) is MenuPageItem pageItem) {
      yield return pageItem;
    }
  }

  private static MenuPageItem? ResolveSingleItem(string modId, RawData dataItem) {
    if (GetPageItemFromConfig(dataItem) is MenuPageItem pageItemFromConfig) {
      return pageItemFromConfig;
    } else if (GetPageItemFromTemplate(dataItem) is MenuPageItem pageItemFromTemplate) {
      return pageItemFromTemplate;
    } else if (GetBasicPageItem(dataItem) is MenuPageItem basicPageItem) {
      return basicPageItem;
    } else {
      Log.Error($"A menu item in mod '{modId}' is improperly defined and will not be displayed.");
      return null;
    }
  }

  private static MenuPageItem? GetPageItemFromTemplate(RawData dataItem) {
    // TODO: Properly implement this.
    return (dataItem.FromTemplate is string templateName) ? new MenuPageItem() : null;
  }

  private static MenuPageItem? GetPageItemFromConfig(RawData dataItem) {
    // TODO: Properly implement this.
    return (dataItem.FromConfig is string configName) ? new MenuPageItem() : null;
  }

  private static MenuPageItem? GetBasicPageItem(RawData dataItem) {
    // TODO: Properly implement this.
    return new MenuPageItem();
  }
}
