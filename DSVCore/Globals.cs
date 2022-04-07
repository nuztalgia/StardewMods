using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class Globals {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value.
  internal static IManifest Manifest { get; private set; }
  internal static IModRegistry ModRegistry { get; private set; }
  internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618

  private readonly static Dictionary<string, string> I18nCache = new();

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

  internal static string GetI18nString(string name) {
    if (I18nCache.TryGetValue(name, out string? value)) {
      return value;
    }

    Type type = typeof(I18n.Keys);
    value = (type.GetField(name)?.GetValue(null) is string key) ? I18n.GetByKey(key) : null;

    if (value is null) {
      Log.Error($"No i18n available for '{name}'. Displaying internal name.");
      value = name;
    }

    I18nCache.Add(name, value);
    return value;
  }
}
