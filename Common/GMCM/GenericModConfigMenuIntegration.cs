using System.Diagnostics.CodeAnalysis;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed class GenericModConfigMenuIntegration : BaseIntegration<IGenericModConfigMenuApi> {

  private Action<string, object>? HandleFieldChanged;

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private GenericModConfigMenuIntegration(IGenericModConfigMenuApi api, IManifest manifest)
      : base(api, manifest) { }

  internal GenericModConfigMenuIntegration Register(Action resetAction, Action saveAction) {
    this.Api.Register(this.Manifest, resetAction, saveAction);
    return this;
  }

  internal GenericModConfigMenuIntegration OnFieldChanged(Action<string, object> onFieldChanged) {
    this.HandleFieldChanged = onFieldChanged;
    this.Api.OnFieldChanged(this.Manifest, onFieldChanged);
    return this;
  }

  internal GenericModConfigMenuIntegration AddPage(string pageId, string pageTitle) {
    this.Api.AddPage(this.Manifest, pageId, () => pageTitle);
    return this;
  }

  internal GenericModConfigMenuIntegration AddPageLink(string pageId, string text) {
    this.Api.AddPageLink(this.Manifest, pageId, () => text);
    return this;
  }

  internal GenericModConfigMenuIntegration AddEnumOption(
      object container,
      PropertyInfo property,
      string optionName,
      Func<string, string>? formatValue = null,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddTextOption(
        mod: this.Manifest,
        name: () => optionName,
        getValue: () => property.GetValue(container)?.ToString() ?? string.Empty,
        setValue: value => property.SetValue(container, Enum.Parse(property.PropertyType, value)),
        allowedValues: Enum.GetNames(property.PropertyType),
        formatAllowedValue: formatValue,
        tooltip: (tooltip is null) ? null : () => tooltip,
        fieldId: fieldId
    );

    return this;
  }

  internal GenericModConfigMenuIntegration AddComplexOption(
      string optionName,
      Func<int> getHeight,
      Action<SpriteBatch, Vector2> drawAction,
      Action? resetAction = null,
      Action? saveAction = null,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddComplexOption(
        mod: this.Manifest,
        name: () => optionName,
        draw: drawAction,
        tooltip: (tooltip is null) ? null : () => tooltip,
        beforeMenuOpened: resetAction, // Make sure the option reflects the actual current state.
        beforeMenuClosed: resetAction, // Option state should be reset when the menu page is closed.
        beforeReset: resetAction,
        beforeSave: () => saveAction?.Invoke(), // GMCM doesn't support null actions for beforeSave.
        height: getHeight,
        fieldId: fieldId
    );

    return this;
  }

  internal GenericModConfigMenuIntegration AddSpacing() {
    Spacing.DefaultVertical.AddToConfigMenu(this.Api, this.Manifest);
    return this;
  }

  internal GenericModConfigMenuIntegration AddParagraph(string text) {
    StaticText.CreateParagraph(text).AddToConfigMenu(this.Api, this.Manifest);
    return this;
  }

  internal GenericModConfigMenuIntegration AddParagraph(Func<string> getText) {
    return this.AddParagraph(getText());
  }

  internal GenericModConfigMenuIntegration AddHeader(string text) {
    new Header(text).AddToConfigMenu(this.Api, this.Manifest);
    return this;
  }

  internal GenericModConfigMenuIntegration AddHeader(Func<string> getText) {
    return this.AddHeader(getText());
  }

  internal GenericModConfigMenuIntegration AddHeaderWithButton(
      string headerText, string buttonText, Action buttonAction) {

    new Header.WithButton(headerText, buttonText, buttonAction)
        .AddToConfigMenu(this.Api, this.Manifest);

    return this;
  }

  internal GenericModConfigMenuIntegration AddCheckbox(
      object container,
      PropertyInfo property,
      string optionName,
      string? tooltip = null,
      string? fieldId = null) {

    new Checkbox(
        name: optionName,
        tooltip: tooltip,
        loadValue: () => (bool) property.GetValue(container)!,
        saveValue: (bool value) => property.SetValue(container, value),
        onValueChanged: (fieldId == null) ? null
            : (newValue) => this.HandleFieldChanged?.Invoke(fieldId, newValue)
    ).AddToConfigMenu(this.Api, this.Manifest);

    return this;
  }

  internal GenericModConfigMenuIntegration AddSlider(
      object container,
      PropertyInfo property,
      string optionName,
      string? tooltip = null,
      string? fieldId = null,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? formatValue = null) {

    new Slider(
        name: optionName,
        tooltip: tooltip,
        loadValue: () => (int) property.GetValue(container)!,
        saveValue: (int value) => property.SetValue(container, value),
        onValueChanged: (fieldId == null) ? null
            : (newValue) => this.HandleFieldChanged?.Invoke(fieldId, newValue),
        staticMinValue: staticMinValue,
        staticMaxValue: staticMaxValue,
        getDynamicMinValue: getDynamicMinValue,
        getDynamicMaxValue: getDynamicMaxValue,
        formatValue: formatValue
    ).AddToConfigMenu(this.Api, this.Manifest);

    return this;
  }
}
