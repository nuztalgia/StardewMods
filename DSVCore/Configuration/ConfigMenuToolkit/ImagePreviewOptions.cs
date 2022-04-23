using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal static class ImagePreviewOptions {

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

    private ImmutableDictionary<string, object?>? SavedProperties;
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

      static Texture2D[][] TryLoadImages(LoadImage loadImage, string[][] allImagePaths) {
        return allImagePaths
            .Select(imagePathGroup => imagePathGroup
                .Select(imagePath => imagePath.IsEmpty() ? null : loadImage(imagePath))
                .OfType<Texture2D>().ToArray())
            .ToArray();
      }
    }

    internal void SaveState() {
      this.SavedProperties = this.EphemeralProperties.ToImmutableDictionary();
      this.RefreshImages();
    }

    internal void ResetState() {
      if (this.SavedProperties is not null) {
        this.EphemeralProperties.Clear();
        this.SavedProperties.ForEach((key, value) => this.EphemeralProperties.Add(key, value));
        this.RefreshImages();
      } else {
        Log.Trace("Tried to reset state, but there is no saved state to fall back on. Ignoring.");
      }
    }

    internal void UpdateEphemeralProperty(string propertyKey, object? propertyValue) {
      this.EphemeralProperties[propertyKey] = propertyValue;

      // Avoid loading images repeatedly and prematurely during setup. Wait for the first SaveState.
      if (this.SavedProperties is not null) {
        this.RefreshImages();
      }
    }

    internal void DrawPreview(SpriteBatch sb, Vector2 position) {
      position.Y += StandardMargin;
      bool baselineAdjusted = false;

      TryDraw(this.CurrentPortraits, this.GetPortraitRects, PortraitScale);
      TryDraw(this.CurrentSprites, this.GetSpriteRects, SpriteScale);

      void TryDraw(Texture2D[][]? allImages, GetImageRects? getImageRects, int scale) {
        if ((getImageRects is null) || (allImages is null)
            || (!allImages.Any()) || (!allImages.First().Any())) {
          return;
        }

        foreach (var (rectGroup, imageGroup) in getImageRects(this.CurrentSource).Zip(allImages)) {
          if ((rectGroup.Length != 1) && (rectGroup.Length != imageGroup.Length)) {
            Log.Error($"Mismatch: {rectGroup.Length} rectangles and {imageGroup.Length} images.");
          } else {
            Rectangle mainRect = rectGroup.First();
            position.Y -= baselineAdjusted ? (mainRect.Height * scale) : 0;

            if (rectGroup.Length == 1) {
              imageGroup.ForEach(image => sb.Draw(image, position, mainRect, scale));
            } else if (rectGroup.Length == imageGroup.Length) {
              rectGroup.Zip(imageGroup).ForEach(
                  (rect, image) => sb.Draw(image, position, rect, scale));
            }

            position.X += (mainRect.Width * scale) + (StandardMargin * 2);
            position.Y += mainRect.Height * scale;
            baselineAdjusted = true;
          }
        }
      }
    }

    private void RefreshImages() {
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
  }

  private const int PortraitScale = 3;
  private const int SpriteScale = 5;
  private const int StandardMargin = 16;
  private const int MinimumHeight = 128;

  private static readonly Dictionary<string, CharacterPreview> CharacterPreviews = new();

  private static readonly HashSet<string> AllowedEphemeralProperties = new() {
    "Variant", "Randomization", "SeasonalOutfits",
  };

  internal static void InitializeCharacter(string characterName,
      LoadImage loadGameImage, LoadImage loadModImage,
      GetModImagePaths getModImagePaths, GetGameImagePaths getGameImagePaths,
      GetImageRects? getPortraitRects, GetImageRects? getSpriteRects) {
    Log.Verbose($"Initializing GMCM character section preview for {characterName}.");
    CharacterPreview characterPreview =
        new(loadGameImage, loadModImage, getModImagePaths,
            getGameImagePaths, getPortraitRects, getSpriteRects);
    CharacterPreviews.Add(characterName, characterPreview);
  }

  internal static int GetHeight(string characterName) {
    CharacterPreview? character = CharacterPreviews.Get(characterName);
    int actualHeight = (character is not null)
        ? Math.Max(GetHeight(character.GetPortraitRects, PortraitScale),
            GetHeight(character.GetSpriteRects, SpriteScale))
        : 0;
    return Math.Max(actualHeight, MinimumHeight) + StandardMargin;

    int GetHeight(GetImageRects? getImageRects, int scale) {
      return (getImageRects?.Invoke(character.CurrentSource).First().First().Height * scale) ?? 0;
    }
  }

  internal static void SetFieldValue(string fieldId, object? propertyValue) {
    (string characterName, string propertyKey) = fieldId.Split('_');
    if (AllowedEphemeralProperties.Contains(propertyKey)) {
      CharacterPreviews.Get(characterName)?.UpdateEphemeralProperty(propertyKey, propertyValue);
    }
  }

  internal static void Draw(string characterName, SpriteBatch sb, Vector2 position) {
    CharacterPreviews.Get(characterName)?.DrawPreview(sb, position);
  }

  internal static void SaveState(string characterName) {
    CharacterPreviews.Get(characterName)?.SaveState();
  }

  internal static void ResetState(string characterName) {
    CharacterPreviews.Get(characterName)?.ResetState();
  }

  // Extension method for Dictionary<string, CharacterPreview> to safely/concisely get a character.
  private static CharacterPreview? Get(this Dictionary<string, CharacterPreview> dict, string key) {
    if (!dict.TryGetValue(key, out CharacterPreview? characterPreview)) {
      Log.Error($"Tried to get image preview for character '{key}', but they aren't in the cache.");
    }
    return characterPreview;
  }

#pragma warning disable IDE0051 // Remove "unused" private members. (This is used by SetFieldValue.)
  // Extension method for string[]. Expects exactly two strings in the array.
  private static void Deconstruct(this string[] items, out string first, out string second) {
    first = items.Length > 0 ? items[0] : string.Empty;
    second = items.Length > 1 ? items[1] : string.Empty;
  }
#pragma warning restore IDE0051
}
