namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal sealed class ConfigFieldData {

  internal readonly record struct CheckboxData(
      bool CurrentValue,
      bool DefaultValue);

  internal readonly record struct DropdownData(
      string[] AllowedValues,
      string CurrentValue,
      string DefaultValue);

  internal readonly record struct SliderData(
      int MinValue,
      int MaxValue,
      int CurrentValue,
      int DefaultValue);

  internal readonly record struct TextFieldData(
      string CurrentValue,
      string DefaultValue);

  private readonly CheckboxData? DataIfCheckbox;
  private readonly DropdownData? DataIfDropdown;
  private readonly SliderData? DataIfSlider;
  private readonly TextFieldData? DataIfTextField;

  internal string Key { get; }

  internal ConfigFieldData(
      string key,
      IEnumerable<string>? currentValues,
      IEnumerable<string>? defaultValues,
      IEnumerable<string>? allowedValues,
      bool allowBlankValues,
      bool allowMultipleValues,
      bool isBooleanValue,
      int? numericRangeMin,
      int? numericRangeMax) {

    this.Key = key;

    string currentStringValue = GetStringValue(currentValues);
    string defaultStringValue = GetStringValue(defaultValues);

    if ((allowedValues == null) || allowedValues.IsEmpty() || allowMultipleValues) {
      // TODO: Support "checkbox groups" for when multiple values are allowed.
      this.DataIfTextField = new TextFieldData(currentStringValue, defaultStringValue);
      return;
    }

    if (!allowBlankValues) {
      if (isBooleanValue) {
        this.DataIfCheckbox =
            new CheckboxData(GetBoolValue(currentStringValue), GetBoolValue(defaultStringValue));
        return;
      } else if ((numericRangeMin is int minIntValue) && (numericRangeMax is int maxIntValue)) {
        int defaultIntValue = GetIntValue(defaultStringValue, minIntValue);
        int currentIntValue = GetIntValue(currentStringValue, defaultIntValue);
        this.DataIfSlider =
            new SliderData(minIntValue, maxIntValue, currentIntValue, defaultIntValue);
        return;
      }
    }

    if (allowBlankValues && !allowedValues.Contains(string.Empty)) {
      allowedValues = allowedValues.Prepend(string.Empty);
    }
    this.DataIfDropdown =
        new DropdownData(allowedValues.ToArray(), currentStringValue, defaultStringValue);
    return;

    static string GetStringValue(IEnumerable<string>? values) {
      return values?.FirstOrDefault() ?? string.Empty;
    }

    static bool GetBoolValue(string value) {
      return value.Contains(true.ToString());
    }

    static int GetIntValue(string value, int valueIfInvalid) {
      return int.TryParse(value, out int intValue) ? intValue : valueIfInvalid;
    }
  }

  internal object? GetDataForUI() {
    return this.DataIfCheckbox
        ?? this.DataIfDropdown
        ?? this.DataIfSlider
        ?? (object?) this.DataIfTextField;
  }
}
