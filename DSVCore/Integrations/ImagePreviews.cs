using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ImagePreviews {

  internal delegate Texture2D? LoadImage(string imageLocation);
  internal delegate Rectangle GetImageRect(ContentSource source);

  internal delegate string GetGameImagePath(string imageDirectory);
  internal delegate string GetModImagePath(
      string imageDirectory, IDictionary<string, object?> ephemeralProperties);

  private sealed class CharacterPreview {
    private readonly LoadImage LoadModImage;
    private readonly LoadImage LoadGameImage;
    private readonly Dictionary<string, object?> EphemeralProperties = new();

    internal GetImageRect? GetPortraitRect { get; private init; }
    internal GetImageRect? GetSpriteRect { get; private init; }
    internal ContentSource CurrentSource { get; private set; } = ContentSource.GameContent;

    private Texture2D? CurrentPortrait;
    private Texture2D? CurrentSprite;

    internal CharacterPreview(
        LoadImage loadGameImage, LoadImage loadModImage, 
        GetModImagePath getModImagePath, GetGameImagePath getGameImagePath,
        GetImageRect? getPortraitRect, GetImageRect? getSpriteRect) {
      this.GetPortraitRect = getPortraitRect;
      this.GetSpriteRect = getSpriteRect;

      this.LoadModImage = (imageDirectory) => {
        this.CurrentSource = ContentSource.ModFolder;
        string imagePath = getModImagePath(imageDirectory, this.EphemeralProperties);
        return string.IsNullOrEmpty(imagePath) ? null : TryLoadImage(loadModImage, imagePath);
      };

      this.LoadGameImage = (imageDirectory) => {
        this.CurrentSource = ContentSource.GameContent;
        return TryLoadImage(loadGameImage, getGameImagePath(imageDirectory));
      };

      static Texture2D? TryLoadImage(LoadImage loadImage, string imagePath) {
        try {
          return loadImage(imagePath);
        } catch (ContentLoadException) {
          Log.Error($"Invalid preview image path: '{imagePath}'");
          return null;
        }
      }
    }

    internal void UpdateEphemeralProperty(string propertyKey, object? propertyValue) {
      this.EphemeralProperties[propertyKey] = propertyValue;

      if (this.GetPortraitRect is not null) {
        this.CurrentPortrait = TryLoadImageFromDirectory("Portraits");
      }
      if (this.GetSpriteRect is not null) {
        this.CurrentSprite = TryLoadImageFromDirectory("Characters");
      }

      Texture2D? TryLoadImageFromDirectory(string imageDirectory) {
        return this.LoadModImage(imageDirectory) ?? this.LoadGameImage(imageDirectory);
      }
    }

    internal void DrawPreview(SpriteBatch sb, Vector2 position) {
      position.Y += StandardMargin;

      bool portraitSuccess = TryDraw(
          this.CurrentPortrait, this.GetPortraitRect, PortraitScale,
          afterDraw: (portraitRect) => {
            position.X += (portraitRect.Width * PortraitScale) + (StandardMargin * 2);
            position.Y += portraitRect.Height * PortraitScale;
          });

      bool spriteSuccess = TryDraw(
          this.CurrentSprite, this.GetSpriteRect, SpriteScale,
          beforeDraw: (spriteRect) => {
            if (portraitSuccess) {
              position.Y -= spriteRect.Height * SpriteScale;
            }
          });

      bool TryDraw(Texture2D? image, GetImageRect? getImageRect, int scale,
                   Action<Rectangle>? beforeDraw = null, Action<Rectangle>? afterDraw = null) {
        if ((image is null) || (getImageRect is null)) {
          return false;
        }
        Rectangle rect = getImageRect(this.CurrentSource);
        beforeDraw?.Invoke(rect);
        sb.Draw(image, position, rect, scale);
        afterDraw?.Invoke(rect);
        return true;
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

  internal static void InitializeCharacter(string characterName,
      LoadImage loadGameImage, LoadImage loadModImage,
      GetModImagePath getModImagePath, GetGameImagePath getGameImagePath,
      GetImageRect? getPortraitRect, GetImageRect? getSpriteRect) {
    Log.Verbose($"Initializing GMCM character section preview for {characterName}.");
    CharacterPreview characterPreview = new(loadGameImage, loadModImage, getModImagePath,
                                            getGameImagePath, getPortraitRect, getSpriteRect);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static int GetHeight(string characterName) {
    int actualHeight = CharacterPreviews.TryGetValue(characterName, out CharacterPreview? character)
                       ? Math.Max(GetHeight(character.GetPortraitRect, PortraitScale),
                                  GetHeight(character.GetSpriteRect, SpriteScale)) : 0;
    return Math.Max(actualHeight, MinimumHeight) + StandardMargin;

    int GetHeight(GetImageRect? getImageRect, int scale) {
      return (getImageRect?.Invoke(character.CurrentSource).Height * scale) ?? 0;
    }
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
