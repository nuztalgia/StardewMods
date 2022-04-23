using System.Collections.Generic;
using System.Reflection;
using Nuztalgia.StardewMods.Common.ContentPatcher;
using Nuztalgia.StardewMods.Common.ModRegistry;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCompatSection : BaseMenuSection {

  protected readonly string ModId;
  protected readonly string ModName;
  protected readonly IEnumerable<string> TokenNames;

  protected BaseCompatSection(string modId, string modName, IEnumerable<string> tokenNames) {
    this.ModId = modId;
    this.ModName = modName;
    this.TokenNames = tokenNames;
  }

  internal override sealed void RegisterTokens(Integration contentPatcher) {
    if (this.IsAvailable()) {
      this.RegisterCompatTokens(contentPatcher);
    } else {
      foreach (string tokenName in this.TokenNames) {
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

  protected override sealed string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{this.Name}_{property.Name}");
  }

  protected abstract void RegisterCompatTokens(Integration contentPatcher);
}
