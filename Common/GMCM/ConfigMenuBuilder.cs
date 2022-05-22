using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed record ConfigMenuBuilder(
    string MenuId,
    ConfigMenuBuilder.RegisterDelegate Register,
    ConfigMenuBuilder.AddPageDelegate AddPage,
    Action<string> LogWarning,
    Action<string> LogTrace,
    Action<string> LogVerbose)
        : IConfigMenuBuilder {

  internal delegate void RegisterDelegate(Action onReset, Action onSave);

  internal delegate void AddPageDelegate(string pageId, string pageTitle, MenuPage menuPageWidget);

  private readonly record struct MenuPageContent(string Title, MenuPage Widget);

  private readonly Dictionary<string, MenuPageContent> MenuPageData = new();

  private bool IsPublished = false;

  public IConfigMenuBuilder CreateNewPage(
      out IConfigPageBuilder pageBuilder,
      string pageId,
      string? pageTitle = null) {

    if (pageId.IsEmpty()) {
      this.LogWarning("Page ID must be a non-empty string.");
      throw new ArgumentException($"Invalid page ID.", nameof(pageId));
    }

    if (this.MenuPageData.ContainsKey(pageId)) {
      this.LogWarning("Page IDs must be unique within a single menu.");
      throw new ArgumentException($"Invalid page ID: '{pageId}'", nameof(pageId));
    }

    this.LogVerbose($"Creating new builder for menu page: '{pageId}'.");

    pageBuilder = new ConfigPageBuilder(
        PageId: pageId,
        End: (menuPageWidget) => this.EndPage(menuPageWidget, pageId, pageTitle),
        LogWarning: this.LogWarning,
        LogVerbose: this.LogVerbose);

    return this;
  }

  public void PublishMenu(Action? onReset = null, Action? onSave = null) {

    this.LogVerbose($"Attempting to publish the config menu...");

    if (this.IsPublished) {
      this.LogWarning($"Cannot re-publish an already-published config menu.");
      return;
    }

    this.Register(
        onReset ?? (() =>
            this.LogTrace($"Config was reset, but a reset handler was not provided.")),
        onSave ?? (() =>
            this.LogTrace($"Config was saved, but a save handler was not provided.")));

    this.MenuPageData.ForEach((pageId, pageContent) =>
        this.AddPage(pageId, pageContent.Title, pageContent.Widget));

    this.MenuPageData.Clear();
    this.IsPublished = true;
    this.LogTrace($"Successfully published the config menu!");
  }

  private IConfigMenuBuilder EndPage(MenuPage? menuPageWidget, string pageId, string? pageTitle) {
    if (menuPageWidget == null) {
      this.LogWarning($"Tried to end menu page '{pageId}', but it has already been ended.");
    } else { 
      this.LogVerbose($"Ending menu page '{pageId}' and adding it to the in-progress config menu.");
      this.MenuPageData.Add(pageId, new MenuPageContent(pageTitle ?? pageId, menuPageWidget));
    }
    return this;
  }
}
