using System.Diagnostics.CodeAnalysis;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed class GenericModConfigMenuIntegration : BaseIntegration<IGenericModConfigMenuApi> {

  internal string OptionNamePrefix {
    get => this.OptionNamePrefix_Field;
    set => this.OptionNamePrefix_Field = $"{value.Trim()} ";
  }

  internal string PageLinkPrefix {
    get => this.PageLinkPrefix_Field;
    set => this.PageLinkPrefix_Field = value.Trim();
  }

  internal Action<string, object> OnFieldChanged {
    get => this.OnFieldChanged_Field;
    set {
      this.Api.OnFieldChanged(this.Manifest, value);
      this.OnFieldChanged_Field = value;
    }
  }

  private string OptionNamePrefix_Field = string.Empty;
  private string PageLinkPrefix_Field = string.Empty;

  private Action<string, object> OnFieldChanged_Field =
      (fieldId, newValue) => Log.Verbose($"Field '{fieldId}' was changed to: '{newValue}'.");

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private GenericModConfigMenuIntegration(IGenericModConfigMenuApi api, IManifest manifest)
      : base(api, manifest) { }

  internal GenericModConfigMenuIntegration Register(Action resetAction, Action saveAction) {
    this.Api.Register(this.Manifest, resetAction, saveAction);
    return this;
  }

  internal GenericModConfigMenuIntegration AddPage(string pageId, string pageTitle) {
    this.Api.AddPage(this.Manifest, pageId, () => pageTitle);
    return this;
  }

  internal GenericModConfigMenuIntegration AddPageLink(string pageId, string text) {
    text = FormatLabel(text, this.PageLinkPrefix);
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

    optionName = FormatLabel(optionName, this.OptionNamePrefix);

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

  internal GenericModConfigMenuIntegration AddHeaderWithPrefix(string text, string prefix) {
    return this.AddHeader(FormatLabel(text, prefix));
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
        name: FormatLabel(optionName, this.OptionNamePrefix),
        loadValue: GetLoadValue<bool>(container, property),
        saveValue: GetSaveValue<bool>(container, property),
        onValueChanged: this.GetOnValueChanged<bool>(fieldId),
        tooltip: tooltip
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
        name: FormatLabel(optionName, this.OptionNamePrefix),
        loadValue: GetLoadValue<int>(container, property),
        saveValue: GetSaveValue<int>(container, property),
        onValueChanged: this.GetOnValueChanged<int>(fieldId),
        staticMinValue: staticMinValue,
        staticMaxValue: staticMaxValue,
        getDynamicMinValue: getDynamicMinValue,
        getDynamicMaxValue: getDynamicMaxValue,
        formatValue: formatValue,
        tooltip: tooltip
    ).AddToConfigMenu(this.Api, this.Manifest);

    return this;
  }

  internal void OpenMenuForMod(IManifest modManifest) {
    this.Api.OpenModMenu(modManifest);
  }

  private static string FormatLabel(string labelText, string prefix) {
    return prefix.IsEmpty() ? labelText.Trim() : $" {prefix} {labelText.Trim()}";
  }

  private static Func<TValue> GetLoadValue<TValue>(object container, PropertyInfo property) {
    return () => (TValue) property.GetValue(container)!;
  }

  private static Action<TValue> GetSaveValue<TValue>(object container, PropertyInfo property) {
    return (TValue value) => property.SetValue(container, value);
  }

  private Action<TValue>? GetOnValueChanged<TValue>(string? fieldId) where TValue : notnull {
    return (fieldId != null)
        ? (TValue newValue) => this.OnFieldChanged.Invoke(fieldId, newValue)
        : null;
  }
}
