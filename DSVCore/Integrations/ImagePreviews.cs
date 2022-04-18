using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ImagePreviews {

  internal delegate Texture2D? LoadImage(string imagePath);
  internal delegate Texture2D[][] LoadImages(string imageDirectory);

  internal delegate string[][] GetGameImagePaths(string imageDirectory);
  internal delegate string[][] GetModImagePaths(
      string imageDirectory, IDictionary<string, object?> ephemeralProperties);

  internal delegate Rectangle[][] GetImageRects(ContentSource source);

  private sealed class CharacterPreview {
    private readonly LoadImages LoadModImages;
    private readonly LoadImages LoadGameImages;
    private readonly Dictionary<string, object?> EphemeralProperties = new();

    internal GetImageRects? GetPortraitRects { get; private init; }
    internal GetImageRects? GetSpriteRects { get; private init; }
    internal ContentSource CurrentSource { get; private set; } = ContentSource.GameContent;

    private Texture2D[][]? CurrentPortraits;
    private Texture2D[][]? CurrentSprites;

    internal CharacterPreview(
        LoadImage loadGameImage, LoadImage loadModImage, 
        GetModImagePaths getModImagePaths, GetGameImagePaths getGameImagePaths,
        GetImageRects? getPortraitRects, GetImageRects? getSpriteRects) {
      this.GetPortraitRects = getPortraitRects;
      this.GetSpriteRects = getSpriteRects;

      this.LoadModImages = (imageDirectory) => {
        this.CurrentSource = ContentSource.ModFolder;
        return TryLoadImages(
            loadModImage, getModImagePaths(imageDirectory, this.EphemeralProperties));
      };

      this.LoadGameImages = (imageDirectory) => {
        this.CurrentSource = ContentSource.GameContent;
        return TryLoadImages(loadGameImage, getGameImagePaths(imageDirectory));
      };
    }

    internal void UpdateEphemeralProperty(string propertyKey, object? propertyValue) {
      this.EphemeralProperties[propertyKey] = propertyValue;

      if (this.GetPortraitRects is not null) {
        this.CurrentPortraits = TryLoadImagesFromDirectory("Portraits");
      }

      if (this.GetSpriteRects is not null) {
        this.CurrentSprites = TryLoadImagesFromDirectory("Characters");
      }

      Texture2D[][] TryLoadImagesFromDirectory(string imageDirectory) {
        var modImages = this.LoadModImages(imageDirectory);
        return modImages.First().Any() ? modImages : this.LoadGameImages(imageDirectory);
      }
    }

    internal void DrawPreview(SpriteBatch sb, Vector2 position) {
      position.Y += StandardMargin;

      bool portraitSuccess = TryDraw(
          this.CurrentPortraits, this.GetPortraitRects, PortraitScale,
          afterDraw: (portraitRect) => {
            position.X += (portraitRect.Width * PortraitScale) + (StandardMargin * 2);
            position.Y += portraitRect.Height * PortraitScale;
          });

      bool spriteSuccess = TryDraw(
          this.CurrentSprites, this.GetSpriteRects, SpriteScale,
          beforeDraw: (spriteRect) => {
            if (portraitSuccess) {
              position.Y -= spriteRect.Height * SpriteScale;
            }
          });

      bool TryDraw(Texture2D[][]? allImages, GetImageRects? getImageRects, int scale,
                   Action<Rectangle>? beforeDraw = null, Action<Rectangle>? afterDraw = null) {
        if ((getImageRects is null) || (allImages is null) || !allImages.Any()) {
          return false;
        }
        foreach (var (rectGroup, imageGroup) in getImageRects(this.CurrentSource).Zip(allImages)) {
          beforeDraw?.Invoke(rectGroup.First());
          foreach (var (rect, image) in rectGroup.Zip(imageGroup)) {
            sb.Draw(image, position, rect, scale);
          }
          afterDraw?.Invoke(rectGroup.Last());
        }
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
      GetModImagePaths getModImagePaths, GetGameImagePaths getGameImagePaths,
      GetImageRects? getPortraitRects, GetImageRects? getSpriteRects) {
    Log.Verbose($"Initializing GMCM character section preview for {characterName}.");
    CharacterPreview characterPreview = new(loadGameImage, loadModImage, getModImagePaths,
                                            getGameImagePaths, getPortraitRects, getSpriteRects);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static int GetHeight(string characterName) {
    int actualHeight = CharacterPreviews.TryGetValue(characterName, out CharacterPreview? character)
                       ? Math.Max(GetHeight(character.GetPortraitRects, PortraitScale),
                                  GetHeight(character.GetSpriteRects, SpriteScale)) : 0;
    return Math.Max(actualHeight, MinimumHeight) + StandardMargin;

    int GetHeight(GetImageRects? getImageRects, int scale) {
      return (getImageRects?.Invoke(character.CurrentSource).First().First().Height * scale) ?? 0;
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

  private static Texture2D[][] TryLoadImages(LoadImage loadImage, string[][] allImagePaths) {
    return allImagePaths
        .Select(imagePathGroup => imagePathGroup
            .Select(imagePath => TryLoadImage(loadImage, imagePath))
            .OfType<Texture2D>().ToArray())
        .ToArray();
  }

  private static Texture2D? TryLoadImage(LoadImage loadImage, string imagePath) {
    if (!string.IsNullOrEmpty(imagePath)) {
      try {
        return loadImage(imagePath);
      } catch (ContentLoadException) {
        Log.Error($"Invalid preview image path: '{imagePath}'");
      }
    }
    return null;
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
