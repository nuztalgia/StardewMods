using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ImagePreviews {

  internal delegate string GetImage(string imageDirectory, Dictionary<string, object?> properties);

  private sealed class CharacterPreview {
    private readonly string CharacterName;
    private readonly IModContentHelper ModContent;
    private readonly GetImage GetPreviewImagePath;
    private readonly Dictionary<string, object?> EphemeralProperties = new();

    internal Texture2D? CurrentPortrait { get; private set; }
    internal Texture2D? CurrentSprite { get; private set; }

    internal CharacterPreview(
        string characterName, IModContentHelper modContent, GetImage getPreviewImagePath) {
      this.CharacterName = characterName;
      this.ModContent = modContent;
      this.GetPreviewImagePath = getPreviewImagePath;
    }

    internal void UpdateEphemeralProperty(string propertyKey, object? propertyValue) {
      this.EphemeralProperties[propertyKey] = propertyValue;
      this.CurrentPortrait = this.GetImageTexture(PortraitDirectory);
      this.CurrentSprite = this.GetImageTexture(SpriteDirectory);
    }

    internal void DrawPreview(SpriteBatch sb, Vector2 position) {
      if (this.CurrentPortrait is not null) {
        sb.Draw(this.CurrentPortrait, position, PortraitBounds, PortraitScale);
        position.X += (PortraitWidth * PortraitScale) + PreviewMargin;
        position.Y += SpriteHeight;
      }

      if (this.CurrentSprite is not null) {
        sb.Draw(this.CurrentSprite, position, SpriteBounds, SpriteScale);
      }
    }

    private Texture2D? GetImageTexture(string imageDirectory) {
      string imagePath = this.GetPreviewImagePath(imageDirectory, this.EphemeralProperties);
      if (!string.IsNullOrEmpty(imagePath)) {
        try {
          return imagePath.Contains("Off")
                 ? Globals.ContentHelper.Load<Texture2D>($"{imageDirectory}/{this.CharacterName}")
                 : this.ModContent.Load<Texture2D>(imagePath);
        } catch (ContentLoadException) {
          Log.Error($"Invalid preview image path: '{imagePath}'");
        }
      }
      Log.Trace($"No preview image available in '{imageDirectory}' for {this.CharacterName}.");
      return null;
    }
  }

  internal const int PreviewHeight = PortraitHeight * PortraitScale;
  internal const int PreviewMargin = 16;

  private const string PortraitDirectory = "Portraits";
  private const int PortraitWidth = 64;
  private const int PortraitHeight = 64;
  private const int PortraitScale = 3;

  private const string SpriteDirectory = "Characters";
  private const int SpriteWidth = 16;
  private const int SpriteHeight = 32;
  private const int SpriteScale = 5;

  private static readonly Rectangle PortraitBounds = new(0, 0, PortraitWidth, PortraitHeight);
  private static readonly Rectangle SpriteBounds = new(0, 0, SpriteWidth, SpriteHeight);

  private static readonly Dictionary<string, CharacterPreview> CharacterPreviews = new();

  private static readonly HashSet<string> AllowedEphemeralProperties = new() {
    "Variant", "SeasonalOutfits",
  };

  internal static void InitializeCharacter(
      IModContentHelper modContent, string characterName, GetImage getPreviewImagePath) {
    Log.Verbose($"Initializing GMCM character section preview for {characterName}.");
    CharacterPreview characterPreview = new(characterName, modContent, getPreviewImagePath);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static void SetFieldValue(string fieldId, object? propertyValue) {
    (string characterName, string propertyKey) = fieldId.Split('_');
    if (AllowedEphemeralProperties.Contains(propertyKey)
        && CharacterPreviews.TryGetValue(characterName, out CharacterPreview? character)) {
      // TODO: All ephemeral properties should be set to their defaults when menu is opened/closed.
      character.UpdateEphemeralProperty(propertyKey, propertyValue);
    }
  }

  internal static void Draw(string characterName, SpriteBatch sb, Vector2 position) {
    CharacterPreviews.TryGetValue(characterName, out CharacterPreview? character);
    character?.DrawPreview(sb, position);
  }

  // Extension method for SpriteBatch. Uses default values for irrelevant/unused parameters.
  private static void Draw(this SpriteBatch sb,
      Texture2D texture, Vector2 position, Rectangle sourceRect, float scale) {
    sb.Draw(texture, position, sourceRect, color: Color.White, rotation: 0f,
            origin: Vector2.Zero, scale, effects: SpriteEffects.None, layerDepth: 1f);
  }

#pragma warning disable IDE0051 // Remove "unused" private members. (This is used by SetFieldValue.)
  // Extension method for string[]. Expects exactly two strings in the array.
  private static void Deconstruct(this string[] items, out string t0, out string t1) {
    t0 = items.Length > 0 ? items[0] : string.Empty;
    t1 = items.Length > 1 ? items[1] : string.Empty;
  }
#pragma warning restore IDE0051
}
