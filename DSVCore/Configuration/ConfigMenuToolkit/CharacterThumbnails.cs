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

  private const int RawIconSize = 13;
  private const int IconScale = 2;
  private const int TicksPerIconFrame = 7;
  private const int NumberOfIconFrames = 4;
  private const int MessagePadding = 42;

  private const int ItemPadding = 8;
  private const int RowPadding = 48;
  private const int RawImageWidth = 64;
  private const int RawImageHeight = 64;
  private const int ImageScale = 2;

  private const int ScaledImageWidth = RawImageWidth * ImageScale;
  private const int ScaledImageHeight = RawImageHeight * ImageScale;

  private static readonly Rectangle ImageRect = new(0, 0, RawImageWidth, RawImageHeight);
  private static readonly Rectangle EmptyStateRect = new(540, 333, RawIconSize, RawIconSize);
  private static readonly Rectangle ErrorStateRect = new(592, 346, RawIconSize, RawIconSize);

  private static readonly SpriteFont MessageFont = Game1.smallFont;
  private static readonly int MessageLineHeight = MessageFont.MeasureLineHeight();

  private static readonly SpriteFont LabelFont = Game1.dialogueFont;
  private static readonly Vector2 MaximumLabelSize = LabelFont.MeasureString("Sebastian");

  private static readonly int ItemWidth =
      Math.Max(ScaledImageWidth, (int) MaximumLabelSize.X) + (ItemPadding * 2);
  private static readonly int ItemHeight =
      ScaledImageHeight + ItemPadding + ((int) MaximumLabelSize.Y) + RowPadding;

  private static int AvailableWidth => Math.Min(1200, Game1.uiViewport.Width - 200);
  private static int NumberOfItemsPerRow => AvailableWidth / ItemWidth;

  private readonly GetCharacterNames GetNames;
  private readonly GetImageData GetImages;
  private readonly string EmptyStateMessage;
  private readonly string ErrorStateMessage;

  private readonly Dictionary<string, Texture2D[]> CharacterImageData = new();
  private readonly Texture2D IconSourceImage;

  private List<string>? CurrentCharacters = null;
  private int CurrentIconTick = 0;
  private int CurrentIconFrame = 0;

  internal CharacterThumbnails(
      GetCharacterNames getCharacterNames,
      GetImageData getImageData,
      CharacterConfigState.LoadImage loadGameImage,
      string emptyStateMessage,
      string errorStateMessage) {

    this.GetNames = getCharacterNames;
    this.GetImages = getImageData;
    this.EmptyStateMessage = emptyStateMessage;
    this.ErrorStateMessage = errorStateMessage;

    this.IconSourceImage = loadGameImage("LooseSprites/Cursors")!;
  }

  internal void Update() {
    this.CharacterImageData.Clear();
    this.CurrentCharacters = this.GetNames()?.OrderBy(characterName => characterName).ToList();

    // TODO: Add some sort of indicator for characters whose content packs aren't installed.
    this.CurrentCharacters?.ForEach(characterName => {
      if (this.GetImages(characterName) is Texture2D[] imageData) {
        this.CharacterImageData.Add(characterName, imageData);
      }
    });
  }

  internal int GetHeight() {
    int numberOfCharacters = this.CharacterImageData.Count;
    if (numberOfCharacters == 0) {
      string msg = this.CurrentCharacters is null ? this.ErrorStateMessage : this.EmptyStateMessage;
      int numberOfLines = MessageFont.GetLines(msg, 0, AvailableWidth).Count();
      return (MessageLineHeight * (numberOfLines + 2)) + ItemPadding;
    } else {
      int numberOfRows = (int) Math.Ceiling(numberOfCharacters / (float) NumberOfItemsPerRow);
      return (numberOfRows * ItemHeight) + (ItemPadding * 3);
    }
  }

  internal void Draw(SpriteBatch sb, Vector2 position) {
    float startX = position.X - (AvailableWidth / 2);
    float endX = startX + AvailableWidth;

    if (this.CharacterImageData.Any()) {
      position.X = startX =
          GetCenteredDrawPosition(NumberOfItemsPerRow * ItemWidth, startX, endX) - ItemPadding;
      position.Y += ItemPadding;

      foreach ((string characterName, Texture2D[] imageData) in this.CharacterImageData) {
        if ((position.X + ItemWidth) > endX) {
          position.X = startX;
          position.Y += ItemHeight;
        }

        DrawThumbnailItem(sb, position, characterName, imageData);
        position.X += ItemWidth;
      }
    } else {
      bool isError = this.CurrentCharacters is null;

      position.X = startX;
      this.DrawIcon(sb, position, isError ? ErrorStateRect : EmptyStateRect);

      position.X += MessagePadding;
      string message = isError ? this.ErrorStateMessage : this.EmptyStateMessage;

      foreach (string line in MessageFont.GetLines(message, position.X, endX)) {
        sb.DrawString(line, position, MessageFont, drawShadow: true);
        position.Y += MessageLineHeight;
      }
    }
  }

  private void DrawIcon(SpriteBatch sb, Vector2 position, Rectangle sourceRect) {
    this.CurrentIconTick = (this.CurrentIconTick + 1) % TicksPerIconFrame;

    if (this.CurrentIconTick == 0) {
      this.CurrentIconFrame = (this.CurrentIconFrame + 1) % NumberOfIconFrames;
    }

    if (this.CurrentIconFrame != 0) {
      sourceRect.X += this.CurrentIconFrame * RawIconSize;
    }

    sb.Draw(this.IconSourceImage, position, sourceRect, IconScale);
  }

  private static void DrawThumbnailItem(
      SpriteBatch sb, Vector2 position, string characterName, Texture2D[] imageData) {
    (float startX, float endX) = (position.X, position.X + ItemWidth);

    Vector2 imagePosition = new(
        x: GetCenteredDrawPosition(ScaledImageWidth, startX, endX),
        y: position.Y);
    Vector2 textPosition = new(
        x: GetCenteredDrawPosition((int) LabelFont.MeasureString(characterName).X, startX, endX),
        y: position.Y + ScaledImageHeight + ItemPadding);

    imageData.ForEach(image => sb.Draw(image, imagePosition, ImageRect, ImageScale));
    sb.DrawString(characterName, textPosition);
  }

  private static int GetCenteredDrawPosition(int size, float startBound, float endBound) {
    return (int) (startBound + ((endBound - startBound - size) / 2));
  }
}
