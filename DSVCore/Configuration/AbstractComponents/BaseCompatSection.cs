using System.Collections.Generic;
using System.Reflection;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCompatSection : BaseMenuSection {

  protected readonly string ModId;
  protected readonly string ModName;

  protected BaseCompatSection(string modId, string modName) {
    this.ModId = modId;
    this.ModName = modName;
  }

  internal override sealed void RegisterTokens(ContentPatcherIntegration contentPatcher) {
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
    return Globals.ModRegistry.IsLoaded(this.ModId);
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{this.Name}_{property.Name}");
  }

  protected abstract void RegisterAllTokens(ContentPatcherIntegration contentPatcher);

  protected abstract IEnumerable<string> GetTokenNames();
}
