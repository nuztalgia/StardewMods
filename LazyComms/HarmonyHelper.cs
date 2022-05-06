using System;
using HarmonyLib;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class HarmonyHelper {

  private const string PatchClassName = "StardewModdingAPI.Framework.CommandManager";
  private const string PatchMethodName = "TryParse";

  internal static void Patch(string id) {
    try {
      new Harmony(id).Patch(
          original: AccessTools.Method(
              typeof(StardewModdingAPI.Mod).Assembly.GetType(PatchClassName), PatchMethodName),
          prefix: new HarmonyMethod(typeof(HarmonyHelper), nameof(CommandManager_TryParse_Prefix))
      );
    } catch (Exception exception) {
      Log.Error($"Failed to apply Harmony patch. Technical details:\n{exception}");
    }
  }

  private static bool CommandManager_TryParse_Prefix(ref string input) {
    string translatedInput = Utilities.TranslateInput(input);
    if (input != translatedInput) {
      Log.Info($"Received command '{input}'. Executing command '{translatedInput}'.");
      input = translatedInput;
    }
    return true;
  }
}
