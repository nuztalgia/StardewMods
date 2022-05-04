using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewValley;

namespace Nuztalgia.StardewMods.DSVCore;

internal class CharacterThumbnails {

  internal delegate IEnumerable<string>? GetCharacterNames();
  internal delegate Texture2D[] GetImageData(string characterName);

  private readonly record struct Measurement(int Width, int Height);

  private const int ItemPadding = 8;
  private const int RowPadding = 48;
  private const int RawImageWidth = 64;
  private const int RawImageHeight = 64;
  private const int ImageScale = 2;

  private const int ScaledImageWidth = RawImageWidth * ImageScale;
  private const int ScaledImageHeight = RawImageHeight * ImageScale;

  private static readonly Rectangle ImageRect = new(0, 0, RawImageWidth, RawImageHeight);
  private static readonly Measurement MaximumLabelSize = MeasureText("Sebastian"); // Longest name.

  private static readonly int ItemWidth =
      Math.Max(ScaledImageWidth, MaximumLabelSize.Width) + (ItemPadding * 2);
  private static readonly int ItemHeight =
      ScaledImageHeight + MaximumLabelSize.Height + ItemPadding + RowPadding;

  private static int AvailableWidth => Math.Min(1200, Game1.uiViewport.Width - 200);

  private readonly GetCharacterNames GetNames;
  private readonly GetImageData GetImages;
  private readonly string EmptyStateMessage;
  private readonly string ErrorStateMessage;

  private readonly Dictionary<string, Texture2D[]> CharacterImageData = new();

  private List<string>? CurrentCharacters = null;

  internal CharacterThumbnails(
      GetCharacterNames getCharacterNames,
      GetImageData getImageData,
      string emptyStateMessage,
      string errorStateMessage) {
    this.GetNames = getCharacterNames;
    this.GetImages = getImageData;
    this.EmptyStateMessage = emptyStateMessage;
    this.ErrorStateMessage = errorStateMessage;
  }

  internal void Update() {
    this.CurrentCharacters = this.GetNames()?.OrderBy(characterName => characterName).ToList();
    this.CurrentCharacters?.ForEach(characterName =>
        this.CharacterImageData[characterName] = this.GetImages(characterName));
  }

  internal int GetHeight() {
    return this.CurrentCharacters?.Count switch {
      null => MeasureText(this.ErrorStateMessage).Height,
      0 => MeasureText(this.EmptyStateMessage).Height,
      int numberOfCharacters => MeasureThumbnails(numberOfCharacters).Height
    } + (ItemPadding * 3);
  }

  internal void Draw(SpriteBatch sb, Vector2 position) {
    if (this.CurrentCharacters?.Any() is null or false) {
      // TODO: Make these messages look pretty too.
      sb.DrawString(
          this.CurrentCharacters is null ? this.ErrorStateMessage : this.EmptyStateMessage,
          position);
      return;
    }

    float endX = position.X + (AvailableWidth / 2);
    float startX = GetCenteredDrawPosition(
        size: MeasureThumbnails(this.CurrentCharacters.Count).Width,
        startBound: endX - AvailableWidth,
        endBound: endX) - ItemPadding;

    position.X = startX;
    position.Y += ItemPadding;

    foreach (string characterName in this.CurrentCharacters) {
      if ((position.X + ItemWidth) > endX) {
        position.X = startX;
        position.Y += ItemHeight;
      }

      this.DrawItem(sb, position, characterName);
      position.X += ItemWidth;
    }
  }

  private void DrawItem(SpriteBatch sb, Vector2 position, string characterName) {
    (float startX, float endX) = (position.X, position.X + ItemWidth);
    Vector2 imagePosition = new(
        x: GetCenteredDrawPosition(ScaledImageWidth, startX, endX),
        y: position.Y);
    Vector2 textPosition = new(
        x: GetCenteredDrawPosition(MeasureText(characterName).Width, startX, endX),
        y: position.Y + ScaledImageHeight + ItemPadding);

    this.CharacterImageData[characterName].ForEach(
        (Texture2D image) => sb.Draw(image, imagePosition, ImageRect, ImageScale));
    sb.DrawString(characterName, textPosition);
  }

  private static Measurement MeasureThumbnails(int numberOfCharacters) {
    int numberOfItemsPerRow = AvailableWidth / ItemWidth;
    int numberOfRows = numberOfCharacters / numberOfItemsPerRow;
    numberOfRows += ((numberOfCharacters % numberOfItemsPerRow) == 0) ? 0 : 1;
    return new(numberOfItemsPerRow * ItemWidth, numberOfRows * ItemHeight);
  }

  private static Measurement MeasureText(string text) {
    Vector2 vector = Game1.dialogueFont.MeasureString(text);
    return new((int) vector.X, (int) vector.Y);
  }

  private static int GetCenteredDrawPosition(int size, float startBound, float endBound) {
    return (int) (startBound + ((endBound - startBound - size) / 2));
  }
}
