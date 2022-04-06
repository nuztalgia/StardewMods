using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuztalgia.StardewMods.LazyComms;

public class ModEntry : Mod {

  public override void Entry(IModHelper helper) {
    Log.Initialize(this.Monitor);
    Aliases.Initialize(helper.Content);

    // Register all of the valid user-defined aliases when the mod is first loaded.
    foreach (KeyValuePair<string, string> entry in Aliases.Data) {
      try {
        string command = Utilities.ExpandString(entry.Value).Stringify();
        helper.ConsoleCommands.Add(entry.Key, $"An alias for '{command}'.", HandleCommand);
      } catch (Exception e) when (e is ArgumentException or FormatException) {
        Log.Warn($"Alias name '{entry.Key}' is invalid. Skipping.");
      }
    }

    // This callback is a local function to reuse helper.ConsoleCommands without extra hassle.
    void HandleCommand(string name, string[] arguments) {
      string[] command = Utilities.ExpandEnumerable(arguments.Prepend(name));
      Log.Info($"Triggering command: '{command.Stringify()}'");
      helper.ConsoleCommands.Trigger(command[0], command[1..]);
    }
  }
}
