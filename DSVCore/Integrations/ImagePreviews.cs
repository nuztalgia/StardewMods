using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ImagePreviews {

  internal delegate Texture2D? LoadImage(string imageLocation);
  internal delegate string GetModImagePath(
      string imageDirectory, IDictionary<string, object?> ephemeralProperties);

  private sealed class CharacterPreview {
    private readonly LoadImage LoadModdedImage;
    private readonly LoadImage LoadUnmoddedImage;
    private readonly Dictionary<string, object?> EphemeralProperties = new();

    internal Rectangle? PortraitRect { get; private init; }
    internal Rectangle? SpriteRect { get; private init; }

    private Texture2D? CurrentPortrait;
    private Texture2D? CurrentSprite;

    internal CharacterPreview(
        string characterName, LoadImage loadGameImage, LoadImage loadModImage, 
        GetModImagePath getModImagePath, Rectangle? portraitRect, Rectangle? spriteRect) {
      this.PortraitRect = portraitRect;
      this.SpriteRect = spriteRect;

      this.LoadModdedImage = (imageDirectory) => {
        string imagePath = getModImagePath(imageDirectory, this.EphemeralProperties);
        return (!string.IsNullOrEmpty(imagePath) && !imagePath.Contains("Off"))
               ? TryLoadImage(loadModImage, imagePath, $"Invalid preview image path: '{imagePath}'")
               : null;
      };

      this.LoadUnmoddedImage = (imageDirectory) => {
        return TryLoadImage(loadGameImage, $"{imageDirectory}/{characterName}",
                            $"No preview image available in {imageDirectory} for {characterName}.");
      };

      static Texture2D? TryLoadImage(LoadImage loadImage, string imagePath, string errorText) {
        try {
          return loadImage(imagePath);
        } catch (ContentLoadException) {
          Log.Error(errorText);
          return null;
        }
      }
    }

    internal void UpdateEphemeralProperty(string propertyKey, object? propertyValue) {
      this.EphemeralProperties[propertyKey] = propertyValue;

      if (this.PortraitRect is not null) {
        this.CurrentPortrait = TryLoadImageFromDirectory("Portraits");
      }
      if (this.SpriteRect is not null) {
        this.CurrentSprite = TryLoadImageFromDirectory("Characters");
      }

      Texture2D? TryLoadImageFromDirectory(string imageDirectory) {
        return this.LoadModdedImage(imageDirectory) ?? this.LoadUnmoddedImage(imageDirectory);
      }
    }

    internal void DrawPreview(SpriteBatch sb, Vector2 position) {
      position.Y += StandardMargin;

      if ((this.PortraitRect is Rectangle portraitRect) && (this.CurrentPortrait is not null)) {
        sb.Draw(this.CurrentPortrait, position, portraitRect, PortraitScale);
        position.X += (portraitRect.Width * PortraitScale) + (StandardMargin * 2);
        position.Y += portraitRect.Height * PortraitScale;
      }

      if ((this.SpriteRect is Rectangle spriteRect) && (this.CurrentSprite is not null)) {
        position.Y -= spriteRect.Height * SpriteScale;
        sb.Draw(this.CurrentSprite, position, spriteRect, SpriteScale);
      }
    }
  }

  private const int PortraitScale = 3;
  private const int SpriteScale = 5;
  private const int StandardMargin = 16;
  private const int MinimumHeight = StandardMargin * 4;

  private static readonly Dictionary<string, CharacterPreview> CharacterPreviews = new();

  private static readonly HashSet<string> AllowedEphemeralProperties = new() {
    "Variant", "SeasonalOutfits",
  };

  internal static void InitializeCharacter(
      string characterName, LoadImage loadGameImage, LoadImage loadModImage, 
      GetModImagePath getModImagePath, Rectangle? portraitRect, Rectangle? spriteRect) {
    Log.Verbose($"Initializing GMCM character section preview for {characterName}.");
    CharacterPreview characterPreview =
        new(characterName, loadGameImage, loadModImage, getModImagePath, portraitRect, spriteRect);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static int GetHeight(string characterName) {
    CharacterPreviews.TryGetValue(characterName, out CharacterPreview? character);
    int portraitHeight = (character?.PortraitRect?.Height * PortraitScale) ?? 0;
    int spriteHeight = (character?.SpriteRect?.Height * SpriteScale) ?? 0;
    return StandardMargin + Math.Max(MinimumHeight, Math.Max(portraitHeight, spriteHeight));
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
