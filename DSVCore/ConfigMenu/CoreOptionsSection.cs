using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class CoreOptionsSection : BaseMenuSection {

  internal enum PyjamaHabits {
    Pyjamas,
    NoPyjamas,
    Marriage
  }

  public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.Pyjamas;
  public bool MermaidPendants { get; set; } = true;
  public bool MaternitySprites { get; set; } = false;

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    this.AddTokenByProperty(tokenMap, nameof(this.MermaidPendants), customPrefix: "");
    this.AddTokenByProperty(tokenMap, nameof(this.MaternitySprites), customPrefix: "");

    tokenMap.Add(nameof(this.Pyjamas), () => {
      return WrapTokenValue(this.Pyjamas switch {
        PyjamaHabits.Pyjamas => "True",
        PyjamaHabits.NoPyjamas => "False",
        PyjamaHabits.Marriage => "Marriage",
        _ => "False",
      });
    });
  }

  internal override string GetDisplayName() {
    return I18n.Core_Section_Title();
  }

  internal override bool IsAvailable() {
    // This section is part of the core mod (a.k.a. this mod), so it's always available.
    return true;
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return Globals.GetI18nString($"Tooltip_{property.Name}");
  }
}
