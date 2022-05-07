using System.Collections.Generic;
using System.Linq;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class ConfigHelper {

  private static readonly Dictionary<string, string> Config = new();

  internal static void Initialize(IModHelper modHelper, Integration? gmcm) {
    LoadConfig(modHelper);

    if (gmcm is null) {
      Log.Trace("Could not hook into GMCM. Must relaunch game in order to modify aliases.");
      return;
    }

    gmcm.Register(
        resetAction: () => LoadConfig(modHelper),
        saveAction: () => modHelper.WriteConfig(Config)
    );

    SetUpConfigMenu(gmcm);
  }

  internal static string? GetAliasValue(string key) {
    Config.TryGetValue(key, out string? value);
    return value;
  }

  private static void LoadConfig(IModHelper modHelper) {
    Config.Clear();
    modHelper.ReadConfig<Dictionary<string, string>>()
        .ForEach((key, value) => AddAliasIfValid(key.ToLower(), value));
  }

  private static void AddAliasIfValid(string key, string value) {
    if (string.IsNullOrWhiteSpace(key)) {
      Log.Warn("Cannot register an alias with an empty name. Skipping.");
    } else if (string.IsNullOrWhiteSpace(value)) {
      Log.Warn($"Alias '{key}' doesn't define a valid command. Skipping.");
    } else {
      Log.Trace($"Using alias '{key}' for '{Utilities.TranslateInput(value)}'.");
      Config.Add(key, value);
    }
  }

  private static void SetUpConfigMenu(Integration gmcm) {
    // TODO: Figure out a better format for displaying these.
    gmcm.AddSectionTitle(I18n.CommandAliases)
        .AddParagraph(string.Join(" \n ", Config.Select(alias => $"{alias.Key} > {alias.Value}")));
  }
}
