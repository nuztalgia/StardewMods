using ContentPatcher;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal class TokenRegistry {

  private const string ModId = "Pathoschild.ContentPatcher";

  private readonly IContentPatcherAPI? API;
  private readonly IManifest Manifest;

  internal TokenRegistry(IModRegistry modRegistry, IManifest manifest) {
    this.API = modRegistry.GetApi<IContentPatcherAPI>(ModId);
    this.Manifest = manifest;
  }

  internal void InitializeTokens() {
    if (this.API is null) {
      Log.Error("Could not retrieve Content Patcher API. This mod will not work.");
      return;
    }
    // TODO: Initialize tokens.
  }
}
