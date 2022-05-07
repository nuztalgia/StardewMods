using System;
using System.Reflection;
using HarmonyLib;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class HarmonyHelper {

  private readonly record struct PatchInfo(
      string QualifiedClassName,
      string OriginalMethodName,
      string? PrefixMethodName = null,
      string? PostfixMethodName = null
  );

  private static readonly PatchInfo[] Patches = new PatchInfo[] {
    new("StardewModdingAPI.Framework.CommandManager", "TryParse",
        PrefixMethodName: nameof(SmapiCommandManager_TryParse_Prefix)),
    new("ChatCommands.CommandValidator", "IsValidCommand",
        PrefixMethodName: nameof(ChatCommandsValidator_IsValidCommand_Prefix)),
    new("ChatCommands.Util.Utils", "ParseArgs",
        PrefixMethodName: nameof(ChatCommandsUtils_ParseArgs_Prefix)),
  };

  internal static void Patch(string modId) {
    Harmony harmony = new(modId);

    foreach (PatchInfo patch in Patches) {
      string className = patch.QualifiedClassName;
      string methodName = patch.OriginalMethodName;

      if (AccessTools.TypeByName(className) is not Type classType) {
        Log.Trace($"Couldn't find type of class named '{className}'. Skipping patch.");
        continue;
      }

      if (AccessTools.Method(classType, methodName) is not MethodInfo originalMethod) {
        Log.Trace($"Couldn't find method '{methodName}' on '{className}'. Skipping patch.");
        continue;
      }

      if (patch.PrefixMethodName is string prefixName) {
        TryPatch(prefixName, (patchMethod) => harmony.Patch(originalMethod, prefix: patchMethod));
      }

      if (patch.PostfixMethodName is string postfixName) {
        TryPatch(postfixName, (patchMethod) => harmony.Patch(originalMethod, postfix: patchMethod));
      }
    }
  }

  private static void TryPatch(string patchMethodName, Action<HarmonyMethod> applyPatch) {
    try {
      applyPatch(new HarmonyMethod(typeof(HarmonyHelper), patchMethodName));
    } catch (Exception exception) {
      Log.Error($"Failed to apply patch '{patchMethodName}'. Technical details:\n{exception}");
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
