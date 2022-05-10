using System;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.ModRegistry;
using Nuztalgia.StardewMods.Common.Patching;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class PatchHelper {

  private const string ChatCommandsModId = "cat.chatcommands";

  internal static void ApplyPatches(string modId) {
    HarmonyPatcher patcher = new HarmonyPatcher(modId);
    Type type = typeof(PatchHelper);

    patcher.ForMethod("StardewModdingAPI.Framework.CommandManager", "TryParse")?
        .ApplyPrefixPatch(type, nameof(SmapiCommandManager_TryParse_Prefix));

    if (ModRegistry.IsLoaded(ChatCommandsModId)) {
      patcher.ForMethod("ChatCommands.CommandValidator", "IsValidCommand")?
          .ApplyPrefixPatch(type, nameof(ChatCommandsValidator_IsValidCommand_Prefix));
      patcher.ForMethod("ChatCommands.Util.Utils", "ParseArgs")?
          .ApplyPrefixPatch(type, nameof(ChatCommandsUtils_ParseArgs_Prefix));
    }
  }

  private static bool SmapiCommandManager_TryParse_Prefix(ref string input) {
    input = Translate(input, logDiff: true);
    return true;
  }

  private static bool ChatCommandsValidator_IsValidCommand_Prefix(ref string input) {
    input = Translate(input, logDiff: false);
    return true;
  }

  private static bool ChatCommandsUtils_ParseArgs_Prefix(ref string input) {
    // TODO: Make this log the diff to the in-game chatbox (in addition to the SMAPI console).
    input = Translate(input, logDiff: true);
    return true;
  }

  private static string Translate(string input, bool logDiff) {
    string translatedInput = InputHelper.Translate(input);
    if (logDiff && (input != translatedInput)) {
      Log.Info($"Received command '{input}'. Executing command '{translatedInput}'.");
    }
    return translatedInput;
  }
}
