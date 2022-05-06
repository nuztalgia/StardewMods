using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class Aliases {

  private const string FileName = "aliases.json";

  // This dictionary is only populated once (when the mod is first loaded).
  internal static Dictionary<string, string> Data { get; } = new();

  internal static void Initialize(IModContentHelper contentHelper) {
    try {
      AddSanitizedData(contentHelper.Load<Dictionary<string, string>>(FileName));
    } catch (ContentLoadException) {
      Log.Warn($"Could not find a valid file named '{FileName}' in the mod folder. " +
          $"If it exists, it's formatted incorrectly. Will not add any aliases.");
    }
  }

  private static void AddSanitizedData(Dictionary<string, string> data) {
    foreach ((string aliasKey, string aliasValue) in data) {
      if (aliasKey.IsEmpty()) {
        Log.Warn("Cannot register an alias with an empty name. Skipping.");
      } else if (aliasValue.IsEmpty()) {
        Log.Warn($"Alias '{aliasKey}' doesn't define a valid command. Skipping.");
      } else {
        Data.Add(aliasKey, aliasValue);
      }
    }
  }
}
