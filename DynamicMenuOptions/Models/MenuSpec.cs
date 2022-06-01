namespace Nuztalgia.StardewMods.DynamicMenuOptions;

public sealed record MenuSpec(IEnumerable<MenuPageItem> MenuPageItems) {

  public sealed class RawData {
    // Exactly ONE of the following fields MUST be set to determine how this menu is loaded.
    public List<MenuPageItem.RawData>? MenuPage;
    public string? MenuPageFile;
  }

  internal static MenuSpec ResolveData(string modId, RawData data) {
    List<MenuPageItem.RawData>? menuPageData = data.MenuPage;

    if (menuPageData == null) {
      if (data.MenuPageFile is string fileName) {
        menuPageData =
            ModRegistryUtils.LoadListDataFromContentPack<MenuPageItem.RawData>(modId, fileName);
      } else {
        Log.Error($"Menu for mod '{modId}' is improperly defined and will appear blank in GMCM.");
      }
    }

    return new MenuSpec(
        MenuPageItems: (menuPageData != null)
            ? MenuPageItem.ResolveData(modId, menuPageData)
            : new List<MenuPageItem>());
  }
}
