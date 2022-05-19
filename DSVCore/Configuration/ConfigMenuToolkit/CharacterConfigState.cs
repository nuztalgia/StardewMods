using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal class CharacterConfigState {

  internal delegate Texture2D? LoadImage(string imagePath);
  internal delegate Texture2D[][] LoadImages(string imageDirectory);

  internal delegate string[][] GetGameImagePaths(string imageDirectory);
  internal delegate string[][] GetModImagePaths(
      string imageDirectory, IDictionary<string, object?> ephemeralState);

  internal delegate Rectangle[][] GetImageRects(ContentSource source);

  internal const string PortraitsDirectory = "Portraits";
  internal const string SpritesDirectory = "Characters";

  private static readonly Dictionary<string, CharacterConfigState> CharacterStates = new();

  internal Rectangle[][]? PortraitRects => this.GetPortraitRects?.Invoke(this.CurrentSource);
  internal Rectangle[][]? SpriteRects => this.GetSpriteRects?.Invoke(this.CurrentSource);
  internal Texture2D[][]? CurrentPortraits { get; private set; }
  internal Texture2D[][]? CurrentSprites { get; private set; }

  private readonly GetImageRects? GetPortraitRects;
  private readonly GetImageRects? GetSpriteRects;
  private readonly LoadImages LoadModImages;
  private readonly LoadImages LoadGameImages;
  private readonly Dictionary<string, object?> EphemeralState = new();

  private ImmutableDictionary<string, object?>? SavedState = null;
  private ContentSource CurrentSource = ContentSource.GameContent;

  private CharacterConfigState(
      LoadImage loadGameImage, LoadImage loadModImage, 
      GetModImagePaths getModImagePaths, GetGameImagePaths getGameImagePaths,
      GetImageRects? getPortraitRects, GetImageRects? getSpriteRects) {
    this.GetPortraitRects = getPortraitRects;
    this.GetSpriteRects = getSpriteRects;

    this.LoadModImages = (imageDirectory) => {
      this.CurrentSource = ContentSource.ModFolder;
      return TryLoadImages(loadModImage, getModImagePaths(imageDirectory, this.EphemeralState));
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

  internal static CharacterConfigState Create(
      string characterName,
      LoadImage loadGameImage, LoadImage loadModImage,
      GetModImagePaths getModImagePaths, GetGameImagePaths getGameImagePaths,
      GetImageRects? getPortraitRects, GetImageRects? getSpriteRects,
      IEnumerable<(string fieldId, object? fieldValue)> initialValues) {

    Log.Verbose($"Initializing character config state for {characterName}.");

    CharacterConfigState characterState =
        new(loadGameImage, loadModImage, getModImagePaths,
            getGameImagePaths, getPortraitRects, getSpriteRects);

    CharacterStates.Add(characterName, characterState);
    initialValues.ForEach((string fieldId, object? fieldValue) => Update(fieldId, fieldValue));
    characterState.SaveState();

    return characterState;
  }

  internal static void Update(string fieldId, object? newValue) {
    string[] splitFieldId = fieldId.Split('_');
    (string characterName, string stateKey) = (splitFieldId[0], splitFieldId[1]);
    if (CharacterStates.TryGetValue(characterName, out CharacterConfigState? characterState)) {
      characterState.UpdateEphemeralState(stateKey, newValue);
    }
  }

  internal static Texture2D[]? GetPortraitData(string characterName) {
    return (CharacterStates.TryGetValue(characterName, out CharacterConfigState? characterState)
        && (characterState.CurrentPortraits?.FirstOrDefault() is Texture2D[] portraitData))
            ? portraitData
            : null;
  }

  internal void SaveState() {
    this.SavedState = this.EphemeralState.ToImmutableDictionary();
    this.RefreshImages();
  }

  internal void ResetState() {
    if (this.SavedState is not null) {
      this.EphemeralState.Clear();
      this.SavedState.ForEach((key, value) => this.EphemeralState.Add(key, value));
      this.RefreshImages();
    } else {
      Log.Trace("Tried to reset state, but there is no saved state to fall back on. Ignoring.");
    }
  }

  private void UpdateEphemeralState(string key, object? value) {
    this.EphemeralState[key] = value;

    // Avoid loading images repeatedly and prematurely during setup. Wait for the first SaveState().
    if (this.SavedState is not null) {
      this.RefreshImages();
    }
  }

  private void RefreshImages() {
    if (this.GetPortraitRects is not null) {
      this.CurrentPortraits = TryLoadImagesFromDirectory(PortraitsDirectory);
    }

    if (this.GetSpriteRects is not null) {
      this.CurrentSprites = TryLoadImagesFromDirectory(SpritesDirectory);
    }

    Texture2D[][] TryLoadImagesFromDirectory(string imageDirectory) {
      var modImages = this.LoadModImages(imageDirectory);
      return modImages.First().Any() ? modImages : this.LoadGameImages(imageDirectory);
    }
  }
}
