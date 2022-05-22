using System.Collections.Immutable;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed record ConfigPageBuilder(
    string PageId,
    ConfigPageBuilder.EndDelegate End,
    Action<string> LogWarning,
    Action<string> LogVerbose)
        : IConfigPageBuilder {

  internal delegate IConfigMenuBuilder EndDelegate(MenuPage? menuPageWidget);

  private readonly List<Widget> WidgetsInOrder = new();
  private readonly Dictionary<Widget, Func<bool>?> WidgetsHideWhen = new();

  private bool IsEnded = false;

  public IConfigMenuBuilder EndPage() {
    if (this.IsEnded) {
      return this.End(null);
    }

    Dictionary<Widget, Func<bool>> hideableWidgets = new();

    this.WidgetsHideWhen.ForEach((Widget widget, Func<bool>? hideWhen) => {
      if (hideWhen != null) {
        hideableWidgets.Add(widget, hideWhen);
      }
    });

    MenuPage menuPageWidget = new(
        this.WidgetsInOrder.ToImmutableArray(),
        hideableWidgets.Any() ? hideableWidgets.ToImmutableDictionary() : null);

    this.WidgetsInOrder.Clear();
    this.WidgetsHideWhen.Clear();
    this.IsEnded = true;

    return this.End(menuPageWidget);
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

  public IConfigPageBuilder AddCheckboxOption(
      string name,
      object propertyHolder,
      PropertyInfo propertyInfo,
      Action<bool>? onValueChanged = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddCheckboxOption(
        name: name,
        loadValue: () => ((bool?) propertyInfo.GetValue(propertyHolder)) ?? default,
        saveValue: (value) => propertyInfo.SetValue(propertyHolder, value),
        onValueChanged: onValueChanged,
        tooltip: tooltip,
        hideWhen: hideWhen);
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

  public IConfigPageBuilder AddSliderOption(
      string name,
      object propertyHolder,
      PropertyInfo propertyInfo,
      Action<int>? onValueChanged = null,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddSliderOption(
        name: name,
        loadValue: () => ((int?) propertyInfo.GetValue(propertyHolder)) ?? default,
        saveValue: (value) => propertyInfo.SetValue(propertyHolder, value),
        onValueChanged: onValueChanged,
        staticMinValue: staticMinValue,
        staticMaxValue: staticMaxValue,
        getDynamicMinValue: getDynamicMinValue,
        getDynamicMaxValue: getDynamicMaxValue,
        formatValue: formatValue,
        tooltip: tooltip,
        hideWhen: hideWhen);
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

    return enumType.IsEnum
        ? this.AddDropdownOption(
            name: name,
            allowedValues: Enum.GetNames(enumType),
            loadValue: () => loadValue().ToString() ?? string.Empty,
            saveValue: (value) => saveValue(Parse(value)),
            onValueChanged: (onValueChanged == null) ? null : value => onValueChanged(Parse(value)),
            formatValue: (formatValue == null) ? null : value => formatValue(Parse(value)),
            tooltip: tooltip,
            hideWhen: hideWhen)
        : throw new ArgumentException($"Type '{enumType.Name}' is not an enum type.");

    object Parse(string value) {
      return (Enum.TryParse(enumType, value, out object? result) && (result?.GetType() == enumType))
          ? result
          : throw new FormatException($"Could not parse '{value}' as type '{enumType.Name}'.");
    }
  }

  public IConfigPageBuilder AddDropdownOption(
      string name,
      object propertyHolder,
      PropertyInfo propertyInfo,
      Action<object>? onValueChanged = null,
      Func<object, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) {

    return this.AddDropdownOption(
        name: name,
        enumType: propertyInfo.PropertyType,
        loadValue: () => propertyInfo.GetValue(propertyHolder)?.ToString() ?? string.Empty,
        saveValue: (value) => propertyInfo.SetValue(propertyHolder, value),
        onValueChanged: onValueChanged,
        formatValue: formatValue,
        tooltip: tooltip,
        hideWhen: hideWhen);
  }

  public IConfigPageBuilder AddDropdownOption<TEnum>(
      string name,
      Func<TEnum> loadValue,
      Action<TEnum> saveValue,
      Action<TEnum>? onValueChanged = null,
      Func<TEnum, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null) where TEnum : Enum {

    return this.AddDropdownOption(
        name: name,
        enumType: typeof(TEnum),
        loadValue: () => loadValue(),
        saveValue: (value) => saveValue((TEnum) value),
        onValueChanged: (onValueChanged == null) ? null : value => onValueChanged((TEnum) value),
        formatValue: (formatValue == null) ? null : value => formatValue((TEnum) value),
        tooltip: tooltip,
        hideWhen: hideWhen);
  }

  private IConfigPageBuilder AddWidget(string logName, Widget widget, Func<bool>? hideWhen = null) {
    if (this.IsEnded) {
      this.LogWarning(
          $"Cannot add new items to menu page '{this.PageId}' because it has already been ended.");
    } else {
      this.LogVerbose($"Adding {logName} to in-progress menu page '{this.PageId}'.");
      this.WidgetsInOrder.Add(widget);
      this.WidgetsHideWhen.Add(widget, hideWhen);
    }
    return this;
  }
}
