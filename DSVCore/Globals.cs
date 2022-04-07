using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class Globals {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value.
  internal static IManifest Manifest { get; private set; }
  internal static IModRegistry ModRegistry { get; private set; }
  internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618

  internal static void Initialize(IManifest manifest, IModHelper modHelper) {
    Manifest = manifest;
    ModRegistry = modHelper.ModRegistry;
    Config = modHelper.ReadConfig<ModConfig>();
  }

  internal static void UpdateActiveConfig(object? caller, ModConfig newConfig) {
    if (caller is GenericModConfigMenuIntegration) {
      // Ensure that this is only set from classes that we expect to set it.
      Config = newConfig;
    } else {
      string callerName = caller?.GetType().Name ?? "<unknown>";
      Log.Error($"Failed to update mod config (missing permisson for class '{callerName}').");
    }
  }
}
