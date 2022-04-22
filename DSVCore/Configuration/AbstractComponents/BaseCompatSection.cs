using System.Collections.Generic;
using System.Reflection;
using Nuztalgia.StardewMods.Common.ContentPatcher;
using Nuztalgia.StardewMods.Common.ModRegistry;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCompatSection : BaseMenuSection {

  protected readonly string ModId;
  protected readonly string ModName;

  protected BaseCompatSection(string modId, string modName) {
    this.ModId = modId;
    this.ModName = modName;
  }

  internal override sealed void RegisterTokens(Integration contentPatcher) {
    if (this.IsAvailable()) {
      this.RegisterAllTokens(contentPatcher);
    } else {
      foreach (string tokenName in this.GetTokenNames()) {
        contentPatcher.RegisterConstantToken(tokenName, string.Empty);
      }
    }
  }

  internal override sealed string GetDisplayName() {
    return this.ModName;
  }

  internal override sealed bool IsAvailable() {
    return ModRegistry.IsLoaded(this.ModId);
  }

  internal string GetInfoText() {
    return Globals.GetI18nString($"Info_{this.Name}");
  }

  // Subclasses should override this method if they sync any config options with external mods.
  internal virtual IEnumerable<string>? GetSyncedItems() {
    return null;
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{this.Name}_{property.Name}");
  }

  protected abstract void RegisterAllTokens(Integration contentPatcher);

  protected abstract IEnumerable<string> GetTokenNames();
}
