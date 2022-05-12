using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Nuztalgia.StardewMods.Common.UI;

internal static class SpriteBatchExtensions {

  internal static void DrawFromCursors(
      this SpriteBatch sb,
      Vector2 position,
      Rectangle sourceRect) {

    sb.Draw(Game1.mouseCursors, position, sourceRect);
  }

  internal static void DrawFromCursors(
      this SpriteBatch sb,
      Vector2 position,
      Rectangle sourceRect,
      int width,
      int height,
      Color? color = null,
      bool drawShadow = false) {

    IClickableMenu.drawTextureBox(
        sb, texture: Game1.mouseCursors, sourceRect,
        (int) position.X, (int) position.Y, width, height,
        color ?? Color.White, scale: Game1.pixelZoom, drawShadow);
  }

  internal static void DrawWidget(this SpriteBatch sb, Widget widget, Vector2 position) {
    widget.InternalDraw(sb, position);
  }
}
