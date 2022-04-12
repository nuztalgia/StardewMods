using Nuztalgia.StardewMods.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseMenuPage : BaseMenuComponent {

  internal IEnumerable<BaseMenuSection> GetAllSections() {
    foreach (PropertyInfo property in this.GetType().GetProperties()) {
      if (property.GetValue(this) is BaseMenuSection section) {
        yield return section;
      } else {
        Log.Error($"Invalid type '{property.PropertyType}' for property " +
                  $"'{property.Name}' on page '{this.GetDisplayName()}'.");
      }
    }
  }

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    foreach (BaseMenuSection section in this.GetAllSections()) {
      if (section.IsAvailable()) {
        section.AddTokens(tokenMap);
      }
    }
  }
}
