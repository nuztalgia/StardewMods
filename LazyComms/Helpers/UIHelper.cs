using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class UIHelper {

  internal static readonly int RowHeight =
      RowPadding + Math.Max(KeyLineHeight, SpriteText.getHeightOfString(ArrowText));

  private const string ArrowText = ">";

  private const int RowPadding = 4;
  private const int KeyPadding = 12;
  private const int ArrowPadding = 30;

  private static readonly SpriteFont KeyFont = Game1.dialogueFont;
  private static readonly SpriteFont ValueFont = Game1.smallFont;

  private static readonly int ArrowWidth = SpriteText.getWidthOfString(ArrowText) + ArrowPadding;
  private static readonly int KeyLineHeight = KeyFont.MeasureLineHeight();
  private static readonly int ValueOffsetY = (KeyLineHeight - ValueFont.MeasureLineHeight()) / 2;

  private static int DrawOffsetX => (Math.Min(1200, Game1.uiViewport.Width - 200) / 2) - KeyPadding;

  internal static Action<SpriteBatch, Vector2> GetDrawAction(
      string[] keys, string[] values, int numberOfRows) {
    float maxKeyWidth = keys.Max(key => KeyFont.MeasureString(key).X);

    return (sb, position) => {
      position.X -= DrawOffsetX;
      for (int i = 0; i < numberOfRows; i++) {
        DrawRow(sb, position, maxKeyWidth, keys[i], values[i]);
        position.Y += RowHeight;
      }
    };
  }

  private static void DrawRow(
      SpriteBatch sb, Vector2 position, float maxKeyWidth, string key, string value) {
    float keyWidth = KeyFont.MeasureString(key).X;

    position.X += maxKeyWidth - keyWidth;
    sb.DrawString(key, position);

    position.X += keyWidth + KeyPadding + ArrowPadding;
    SpriteText.drawString(sb, ArrowText, (int) position.X, (int) position.Y);

    position.X += ArrowWidth;
    position.Y += ValueOffsetY;
    sb.DrawString(value, position, ValueFont, drawShadow: true);
  }
}
