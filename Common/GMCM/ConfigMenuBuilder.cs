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

  internal delegate void RegisterDelegate(
      Action onReset, Action onSave, Action<string, object> onFieldChanged);

  internal delegate void AddPageDelegate(
      string pageId, string pageTitle, Widget.MenuPage menuPageWidget);

  private readonly record struct MenuPage(string Title, Widget.MenuPage Widget);

  private readonly Dictionary<string, MenuPage> MenuPages = new();

  private bool IsPublished = false;

  public IConfigMenuBuilder CreateNewPage(
      out IConfigPageBuilder pageBuilder,
      string pageId,
      string? pageTitle = null) {

    if (pageId.IsEmpty()) {
      this.LogWarning("Page ID must be a non-empty string.");
      throw new ArgumentException($"Invalid page ID.", nameof(pageId));
    }

    if (this.MenuPages.ContainsKey(pageId)) {
      this.LogWarning("Page IDs must be unique within a single menu.");
      throw new ArgumentException($"Invalid page ID: '{pageId}'", nameof(pageId));
    }

    this.LogVerbose($"Creating new builder for menu page: '{pageId}'.");

    pageBuilder = new ConfigPageBuilder(
        PageId: pageId,
        Publish: (menuPageWidget) => this.PublishPage(menuPageWidget, pageId, pageTitle),
        LogWarning: this.LogWarning,
        LogVerbose: this.LogVerbose);

    return this;
  }

  public void PublishMenu(
      Action? onReset = null,
      Action? onSave = null,
      Action<string, object>? onFieldChanged = null) {

    this.LogVerbose($"Attempting to publish the config menu...");

    if (this.IsPublished) {
      this.LogWarning($"Cannot re-publish an already-published config menu.");
      return;
    }

    this.Register(
        onReset ?? (() =>
            this.LogTrace($"Config was reset, but a reset handler was not provided.")),
        onSave ?? (() =>
            this.LogTrace($"Config was saved, but a save handler was not provided.")),
        onFieldChanged ?? ((fieldId, newValue) =>
            this.LogVerbose($"Field '{fieldId}' was changed to: '{newValue}'.")));

    this.MenuPages.ForEach((pageId, menuPage) =>
        this.AddPage(pageId, menuPage.Title, menuPage.Widget));

    this.MenuPages.Clear();
    this.IsPublished = true;
    this.LogTrace($"Successfully published the config menu!");
  }

  private IConfigMenuBuilder PublishPage(
      Widget.MenuPage? menuPage, string pageId, string? pageTitle) {
    if (menuPage == null) {
      this.LogWarning($"Cannot re-publish an already-published menu page: '{pageId}'.");
    } else { 
      this.LogVerbose($"Publishing page '{pageId}' and adding it to the in-progress config menu.");
      this.MenuPages.Add(pageId, new MenuPage(pageTitle ?? pageId, menuPage));
    }
    return this;
  }
}
