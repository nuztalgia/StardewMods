using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCharacterSection : BaseMenuSection {

  internal enum StandardVariant {
    Modded,
    Vanilla,
    Off
  }

  internal enum StandardImmersion {
    Full,
    Light,
    Ultralight
  }

  internal enum SimpleImmersion {
    Full,
    Light
  }

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    this.AddTokenByProperty(tokenMap, "Variant");
    this.AddTokenByProperty(tokenMap, "Immersion", customSuffix: "LightweightConfig");
  }

  internal override string GetDisplayName() {
    // To keep things simple, all characters are defined in classes that match their actual names.
    return this.Name;
  }

  internal override bool IsAvailable() {
    // This is only checked if the character's content pack is loaded, so they're always available.
    return true;
  }

  protected override string? GetTooltip(PropertyInfo property) {
    if (property.PropertyType == typeof(StandardImmersion)) {
      return this.FormatCharacterString(I18n.Tooltip_Immersion_Standard);
    } else if (property.PropertyType == typeof(SimpleImmersion)) {
      return this.FormatCharacterString(I18n.Tooltip_Immersion_Simple);
    } else {
      return base.GetTooltip(property);
    }
  }

  protected string FormatCharacterString(Func<string> getString) {
    return string.Format(getString(), this.GetDisplayName());
  }
}
