using System;
using System.Linq;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.LazyComms;

internal class ModEntry : BaseMod {

  protected override void OnModEntry() {
    Aliases.Initialize(this.Helper.ModContent);

    // Register all of the valid user-defined aliases when the mod is first loaded.
    foreach ((string aliasKey, string aliasValue) in Aliases.Data) {
      try {
        string command = Utilities.ExpandString(aliasValue).SpaceJoin();
        this.Helper.ConsoleCommands.Add(aliasKey, $"An alias for '{command}'.", this.HandleCommand);
      } catch (Exception e) when (e is ArgumentException or FormatException) {
        Log.Warn($"Alias name '{aliasKey}' is invalid. Skipping.");
      }
    }
  }

  private void HandleCommand(string name, string[] arguments) {
    string[] command = Utilities.ExpandEnumerable(arguments.Prepend(name)).ToArray();
    Log.Info($"Triggering command: '{command.SpaceJoin()}'");

#pragma warning disable CS0618 // ICommandHelper.Trigger() is obsolete & will be removed in SMAPI 4.
    this.Helper.ConsoleCommands.Trigger(command[0], command[1..]);
#pragma warning restore CS0618
  }
}
