using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;
using StardewValley;

namespace Nuztalgia.StardewMods.DSVCore;

internal class ImagePreviews {

  private const int PortraitWidth = 64;
  private const int PortraitHeight = 64;

  internal static readonly Rectangle PortraitBounds = new(0, 0, PortraitWidth, PortraitHeight);

  private sealed class CharacterPreview {
    private readonly string CharacterName;
    private readonly IModContentHelper ModContent;
    private readonly Func<string> GetPortraitPath;

    internal CharacterPreview(
        string characterName, IModContentHelper modContent, Func<string> getPortraitPath) {
      this.CharacterName = characterName;
      this.ModContent = modContent;
      this.GetPortraitPath = getPortraitPath;
    }

    internal Texture2D? GetPortraitImage() {
      string portraitPath = this.GetPortraitPath();
      if (!string.IsNullOrEmpty(portraitPath)) {
        try {
          return this.ModContent.Load<Texture2D>(portraitPath);
        } catch (ContentLoadException) {
          Log.Error($"Invalid portrait path: '{portraitPath}'");
        }
      }
      Log.Trace($"No preview portrait available for '{this.CharacterName}'.");
      return null;
    }
  }

  private static readonly Dictionary<string, CharacterPreview> CharacterPreviews = new();

  internal static void InitializePortrait(
      IModContentHelper modContent, string characterName, Func<string> getPortraitPath) {
    Log.Trace($"Initializing preview portrait for {characterName}.");
    CharacterPreview characterPreview = new(characterName, modContent, getPortraitPath);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static Texture2D? GetPortraitImage(string characterName) {
    CharacterPreviews.TryGetValue(characterName, out CharacterPreview? characterPreview);
    return characterPreview?.GetPortraitImage();
  }
}
