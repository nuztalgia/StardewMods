using System.Reflection;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCompatSection : BaseMenuSection {

  private readonly string ModId;
  private readonly string ModName;

  protected BaseCompatSection(string modId, string modName) {
    this.ModId = modId;
    this.ModName = modName;
  }

  internal override string GetDisplayName() {
    return this.ModName;
  }

  internal override bool IsAvailable() {
    return Globals.ModRegistry.IsLoaded(this.ModId);
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return Globals.GetI18nString($"Option_{this.Name}_{property.Name}");
  }

  protected static void RegisterDummyTokens(
      ContentPatcherIntegration contentPatcher, params string[] tokenNames) {
    foreach (string tokenName in tokenNames) {
      contentPatcher.RegisterCompositeToken(tokenName, new());
    }
  }
}
