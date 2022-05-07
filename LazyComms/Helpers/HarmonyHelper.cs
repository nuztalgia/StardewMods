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
        PrefixMethodName: nameof(CommandManager_TryParse_Prefix)),
  };

  internal static void Patch(string modId) {
    Harmony harmony = new(modId);

    foreach (PatchInfo patch in Patches) {
      string className = patch.QualifiedClassName;
      string methodName = patch.OriginalMethodName;

      if (AccessTools.TypeByName(className) is not Type classType) {
        Log.Error($"Failed to get type of class '{className}'. Skipping patch.");
        continue;
      }

      if (AccessTools.Method(classType, methodName) is not MethodInfo originalMethod) {
        Log.Error($"Failed to get method '{methodName}' on '{className}'. Skipping patch.");
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

  private static bool CommandManager_TryParse_Prefix(ref string input) {
    string translatedInput = InputHelper.Translate(input);
    if (input != translatedInput) {
      Log.Info($"Received command '{input}'. Executing command '{translatedInput}'.");
      input = translatedInput;
    }
    return true;
  }
}
