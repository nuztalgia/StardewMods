using System.Collections.Immutable;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed record ConfigPageBuilder(
    string PageId,
    ConfigPageBuilder.PublishDelegate Publish,
    Action<string> LogWarning,
    Action<string> LogVerbose)
        : IConfigPageBuilder {

  internal delegate IConfigMenuBuilder PublishDelegate(Widget.MenuPage? menuPageWidget);

  private readonly List<Widget> WidgetsInOrder = new();
  private readonly Dictionary<Widget, Func<bool>?> WidgetsHideWhen = new();

  private bool IsPublished = false;

  public IConfigPageBuilder AddSpacing() {
    return this.AddWidget("vertical spacing", Spacing.DefaultVertical);
  }

  public IConfigPageBuilder AddParagraph(string text) {
    return this.AddWidget("static paragraph text", StaticText.CreateParagraph(text));
  }

  public IConfigPageBuilder AddHeader(string text) {
    return this.AddWidget("static header text", new Header(text));
  }

  public IConfigPageBuilder AddHeaderWithButton(
      string headerText, string buttonText, Action buttonAction) {

    return this.AddWidget(
        "static header text with action button",
        new Header.WithButton(headerText, buttonText, buttonAction));
  }

  public IConfigPageBuilder AddCheckbox(
      string name,
      Func<bool> loadValue,
      Action<bool> saveValue,
      Action<bool>? onValueChanged = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddWidget(
        "boolean checkbox",
        new Checkbox(name, loadValue, saveValue, onValueChanged, tooltip),
        hideWhen);
  }

  public IConfigPageBuilder AddSlider(
      string name,
      Func<int> loadValue,
      Action<int> saveValue,
      Action<int>? onValueChanged = null,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddWidget(
        "integer slider",
        new Slider(
            name, loadValue, saveValue, onValueChanged, staticMinValue, staticMaxValue,
            getDynamicMinValue, getDynamicMaxValue, formatValue, tooltip),
        hideWhen);
  }

  public IConfigMenuBuilder PublishPage() {
    if (this.IsPublished) {
      return this.Publish(null);
    }

    Widget.MenuPage menuPageWidget = new(
        this.WidgetsInOrder.ToImmutableArray(),
        this.WidgetsHideWhen.ToImmutableDictionary()); ;

    this.WidgetsInOrder.Clear();
    this.WidgetsHideWhen.Clear();
    this.IsPublished = true;

    return this.Publish(menuPageWidget);
  }

  private IConfigPageBuilder AddWidget(string logName, Widget widget, Func<bool>? hideWhen = null) {
    if (this.IsPublished) {
      this.LogWarning($"Cannot add new components to already-published menu page '{this.PageId}'.");
    } else {
      this.LogVerbose($"Adding {logName} to in-progress menu page '{this.PageId}'.");
      this.WidgetsInOrder.Add(widget);
      this.WidgetsHideWhen.Add(widget, hideWhen);
    }
    return this;
  }
}
