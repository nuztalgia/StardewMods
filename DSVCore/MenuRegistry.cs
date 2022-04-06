using GenericModConfigMenu;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal class MenuRegistry {

  private const string ModId = "spacechase0.GenericModConfigMenu";

  private readonly IGenericModConfigMenuApi? API;
  private readonly IManifest Manifest;

  internal MenuRegistry(IModRegistry modRegistry, IManifest manifest) {
    this.API = modRegistry.GetApi<IGenericModConfigMenuApi>(ModId);
    this.Manifest = manifest;
  }

  internal void InitializeMenus() {
    if (this.API is null) {
      Log.Warn("Generic Mod Config Menu is not installed. " +
               "Some functionality of this mod will be unavailable.");
      return;
    }
    // TODO: Initialize menus.
  }
}
