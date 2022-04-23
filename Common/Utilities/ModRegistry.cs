using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common.ModRegistry;

internal static class ModRegistry {

#pragma warning disable CS8618 // Non-nullable field must contain non-null value. Set in Initialize.
  internal static IModRegistry SmapiModRegistry { get; private set; }
#pragma warning restore CS8618

  private readonly static Dictionary<string, IModContentHelper?> ContentHelperCache = new();

  internal static void Initialize(IModRegistry modRegistry) {
    SmapiModRegistry = modRegistry;
  }

  internal static bool IsLoaded(string modId) {
    return SmapiModRegistry.IsLoaded(modId);
  }

  internal static Texture2D? LoadImageFromContentPack(string modId, string assetPath) {
    return LoadExternalAsset<IContentPack, Texture2D>(modId, assetPath);
  }

  internal static Dictionary<string, string>? LoadConfigFromContentPack(string modId) {
    return LoadExternalAsset<IContentPack, Dictionary<string, string>>(modId, "config.json");
  }

  private static TAsset? LoadExternalAsset<TMod, TAsset>(string modId, string assetPath)
      where TAsset : notnull {
    if (!ContentHelperCache.ContainsKey(modId)) {
      TMod? mod = GetModThroughSmapi<TMod>(modId);
      ContentHelperCache[modId] =
          (mod as IContentPack)?.ModContent ?? (mod as IMod)?.Helper.ModContent;
    }

    if (ContentHelperCache[modId] is IModContentHelper contentHelper) {
      try {
        return contentHelper.Load<TAsset>(assetPath);
      } catch (ContentLoadException) {
        Log.Debug($"Couldn't load '{typeof(TAsset)}' asset '{assetPath}' from mod '{modId}'.");
      }
    }
    return default;
  }

  // Inspired by:  "This is really bad. Pathos don't kill me."  - kittycatcasey
  private static TMod? GetModThroughSmapi<TMod>(string modId) {
    string modPropertyName = typeof(TMod).Name switch {
      nameof(IMod) => "Mod",
      nameof(IContentPack) => "ContentPack",
      _ => throw new ArgumentException($"Unsupported type for mod registry: '{typeof(TMod).Name}'.")
    };

    if (SmapiModRegistry.Get(modId) is not IModInfo modInfo) {
      Log.Trace($"Tried to fetch mod with ID '{modId}', but that mod isn't installed.");
      return default;
    }

    if (modInfo.GetType().GetProperty(modPropertyName) is not PropertyInfo modProperty) {
      throw new InvalidOperationException(
          $"Mod with ID '{modId}' does not have a property named '{modPropertyName}'.");
    }

    object? modPropertyValue = modProperty.GetValue(modInfo);

    if (modPropertyValue is not TMod requestedMod) {
      Log.Debug($"Unexpected object of type '{modPropertyValue?.GetType().Name}' " +
          $"found in property '{modProperty.Name}' of mod with ID '{modId}'.");
      return default;
    }

    Log.Trace($"Successfully retrieved mod of type {typeof(TMod).Name} with ID '{modId}'.");
    return requestedMod;
  }
}
