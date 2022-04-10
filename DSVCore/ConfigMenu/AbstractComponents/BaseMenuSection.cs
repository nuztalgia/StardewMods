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

  internal virtual int? GetMinValue(PropertyInfo property) {
    return null; // Subclasses should implement this method properly if they expect it to be called.
  }

  internal virtual int? GetMaxValue(PropertyInfo property) {
    return null; // Subclasses should implement this method properly if they expect it to be called.
  }

  protected virtual string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{property.Name}");
  }

  protected virtual string? GetTooltip(PropertyInfo property) {
    return Globals.GetI18nString($"Tooltip_{this.Name}_{property.Name}");
  }
}