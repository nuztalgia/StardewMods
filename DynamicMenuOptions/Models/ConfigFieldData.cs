namespace Nuztalgia.StardewMods.DynamicMenuOptions;

internal sealed class ConfigFieldData {

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

    // TODO: Implement the rest of this.
  }
}
