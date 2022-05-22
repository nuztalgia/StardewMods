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

  public IConfigPageBuilder AddStaticHeader(string text) {
    return this.AddWidget("static header text", new Header(text));
  }

  public IConfigPageBuilder AddStaticHeader(Func<string> getText) {
    return this.AddStaticHeader(getText());
  }

  public IConfigPageBuilder AddStaticText(string text) {
    return this.AddWidget("static body text", StaticText.CreateParagraph(text));
  }

  public IConfigPageBuilder AddStaticText(Func<string> getText) {
    return this.AddStaticText(getText());
  }

  public IConfigPageBuilder AddVerticalSpacing() {
    return this.AddWidget("vertical spacing", Spacing.DefaultVertical);
  }

  public IConfigPageBuilder AddVerticalSpacing(int height) {
    return this.AddWidget(
        $"vertical spacing (custom height: {height})",
        Spacing.CreateVertical(height));
  }

  public IConfigPageBuilder AddStaticHeaderWithButton(
      string headerText,
      string buttonText,
      Action buttonAction) {

    return this.AddWidget(
        "static header text with action button",
        new Header.WithButton(headerText, buttonText, buttonAction));
  }

  public IConfigPageBuilder AddCheckboxOption(
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

  public IConfigPageBuilder AddSliderOption(
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

  public IConfigPageBuilder AddDropdownOption(
      string name,
      IEnumerable<string> allowedValues,
      Func<string> loadValue,
      Action<string> saveValue,
      Action<string>? onValueChanged = null,
      Func<string, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddWidget(
        "dropdown selection",
        new Dropdown(
            name, allowedValues, loadValue, saveValue, onValueChanged, formatValue, tooltip),
        hideWhen);
  }

  public IConfigPageBuilder AddDropdownOption(
      string name,
      Type enumType,
      Func<object> loadValue,
      Action<object> saveValue,
      Action<object>? onValueChanged = null,
      Func<object, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddWidget(
        "dropdown selection",
        Dropdown.CreateFromEnum(
            name, enumType, loadValue, saveValue, onValueChanged, formatValue, tooltip),
        hideWhen);
  }

  public IConfigPageBuilder AddDropdownOption<TEnum>(
      string name,
      Func<TEnum> loadValue,
      Action<TEnum> saveValue,
      Action<TEnum>? onValueChanged = null,
      Func<TEnum, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) where TEnum : Enum {

    return this.AddWidget(
        "dropdown selection",
        Dropdown.CreateFromEnum(name, loadValue, saveValue, onValueChanged, formatValue, tooltip),
        hideWhen);
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
