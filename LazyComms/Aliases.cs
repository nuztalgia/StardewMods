using Microsoft.Xna.Framework.Content;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class Aliases {

  private const string FileName = "aliases.json";
  private const ContentSource FileSource = ContentSource.ModFolder;

  // This dictionary is only populated once (when the mod is first loaded).
  internal static Dictionary<string, string> Data { get; } = new();

  internal static void Initialize(IContentHelper contentHelper) {
    try {
      AddSanitizedData(contentHelper.Load<Dictionary<string, string>>(FileName, FileSource));
    } catch (ContentLoadException) {
      Log.Warn($"Could not find a valid file named '{FileName}' in the mod folder. " +
               $"If it exists, it's formatted incorrectly. Will not add any aliases.");
    }
  }

  private static void AddSanitizedData(Dictionary<string, string> data) {
    foreach (KeyValuePair<string, string> entry in data) {
      if (string.IsNullOrEmpty(entry.Key)) {
        Log.Warn("Cannot register an alias with an empty name. Skipping.");
      } else if (string.IsNullOrEmpty(entry.Value)) {
        Log.Warn($"Alias '{entry.Key}' doesn't define a valid command. Skipping.");
      } else {
        Data.Add(entry.Key, entry.Value);
      }
    }
  }
}
