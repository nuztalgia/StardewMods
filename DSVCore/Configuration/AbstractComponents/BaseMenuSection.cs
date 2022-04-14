using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseMenuSection : BaseMenuComponent {

  internal readonly record struct OptionItem(
    PropertyInfo Property,
    string UniqueId,
    string Name,
    string Tooltip,
    object? Value
  );

  private static readonly Dictionary<string, string> ValueNameLookup = new();

  internal IEnumerable<OptionItem> GetOptions() {
    foreach (PropertyInfo property in this.GetType().GetProperties()) {
      string uniqueId = $"{this.Name}_{property.Name}";
      yield return new OptionItem(
        Property: property,
        UniqueId: uniqueId,
        Name: this.GetOptionName(property) ?? property.Name,
        Tooltip: this.GetTooltip(property) ?? uniqueId,
        Value: property.GetValue(this)
      );
    }
  }

  internal IEnumerable<string> GetAllValueDisplayNames(PropertyInfo property) {
    foreach (string valueName in Enum.GetNames(property.PropertyType)) {
      yield return GetValueDisplayName(valueName);
    }
  }

  internal string GetCurrentValueDisplayName(PropertyInfo property) {
    return (property.GetValue(this)?.ToString() is string valueName)
           ? GetValueDisplayName(valueName)
           : string.Empty;
  }

  internal void SetValueByDisplayName(PropertyInfo property, string displayName) {
    property.SetValue(this, Enum.Parse(property.PropertyType, ValueNameLookup[displayName]));
  }

  internal virtual int GetMinValue(PropertyInfo property) {
    return int.MinValue; // Subclasses should implement this properly if they expect to use it.
  }

  internal virtual int GetMaxValue(PropertyInfo property) {
    return int.MaxValue; // Subclasses should implement this properly if they expect to use it.
  }

  protected virtual string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{property.Name}");
  }

  protected virtual string? GetTooltip(PropertyInfo property) {
    return Globals.GetI18nString($"Tooltip_{this.Name}_{property.Name}");
  }

  private static string GetValueDisplayName(string valueName) {
    string displayName = Globals.GetI18nString($"Value_{valueName}") ?? valueName;
    ValueNameLookup[displayName] = valueName; // Allows retrieval of the original value name later.
    return displayName;
  }
}
