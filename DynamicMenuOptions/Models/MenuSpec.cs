namespace Nuztalgia.StardewMods.DynamicMenuOptions;

public sealed record MenuSpec(List<MenuSpec.PageItem> MenuPage) {

  public sealed class RawData {
    // Exactly ONE of the following fields MUST be set to determine how this menu is loaded.
    public List<PageItem>? MenuPage;
    public string? MenuPageFile;
  }

  public sealed class PageItem {
    // TODO: Implement this.
  }

  internal static MenuSpec ResolveData(string modId, RawData data) {
    List<PageItem>? menuPage = data.MenuPage;

    if (menuPage == null) {
      if (data.MenuPageFile != null) {
        menuPage = ModRegistryUtils.LoadListDataFromContentPack<PageItem>(modId, data.MenuPageFile);
      } else {
        Log.Error($"[{modId}] Menu is not properly defined and will appear blank. " +
            "(To fix this, specify either a MenuPage or a MenuPageFile.)");
      }
    }

    return new MenuSpec(MenuPage: menuPage ?? new List<PageItem>());
  }
}
