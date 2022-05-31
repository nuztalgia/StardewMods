namespace Nuztalgia.StardewMods.LazyComms;

internal static class PatchHelper {

  private const string ChatCommandsModId = "cat.chatcommands";

  internal static void ApplyPatches(string modId) {
    HarmonyPatcher patcher = new (modId, typeof(PatchHelper));

    patcher.ForMethod("StardewModdingAPI.Framework.CommandManager", "TryParse")?
        .ApplyPrefixPatch(nameof(CommandManager_TryParse_Prefix));

    patcher.ForMethod("StardewModdingAPI.Framework.ModHelpers.CommandHelper", "Trigger")?
        .ApplyPrefixPatch(nameof(CommandHelper_Trigger_Prefix));

    if (ModRegistryUtils.IsLoaded(ChatCommandsModId)) {
      patcher.ForMethod("ChatCommands.CommandValidator", "IsValidCommand")?
          .ApplyPrefixPatch(nameof(CommandValidator_IsValidCommand_Prefix));
    }
  }

  private static bool CommandManager_TryParse_Prefix(ref string input) {
    string translatedInput = InputHelper.Translate(input);

    if (input != translatedInput) {
      Log.Info($"Received command '{input}'. Executing command '{translatedInput}'.");
      input = translatedInput;
    }

    return true;
  }

  private static bool CommandHelper_Trigger_Prefix(ref string name, ref string[] arguments) {
    IEnumerable<string> originalCommand = arguments.Prepend(name);
    IEnumerable<string> translatedCommand = InputHelper.ExpandEnumerable(originalCommand);

    if (!originalCommand.SequenceEqual(translatedCommand)) {
      Log.Info($"Received command '{originalCommand.SpaceJoin()}'. " +
          $"Executing command '{translatedCommand.SpaceJoin()}'.");
      (name, arguments) = (translatedCommand.First(), translatedCommand.Skip(1).ToArray());
    }

    return true;
  }

  private static bool CommandValidator_IsValidCommand_Prefix(ref string input) {
    input = InputHelper.Translate(input);
    return true;
  }
}
