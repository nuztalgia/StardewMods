using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  protected const int PixelZoom = Game1.pixelZoom;

  protected static void Draw(
      SpriteBatch sb, Vector2 position, Texture2D texture, Rectangle sourceRect, int scale) {
    sb.Draw(
        texture, position, sourceRect, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale, effects: SpriteEffects.None, layerDepth: 1f);
  }

  protected static void DrawFromCursors(SpriteBatch sb, Vector2 position, Rectangle sourceRect) {
    Draw(sb, position, texture: Game1.mouseCursors, sourceRect, scale: PixelZoom);
  }

  protected void DrawFromCursors(
      SpriteBatch sb,
      Vector2 position,
      Rectangle sourceRect,
      int? width = null,
      int? height = null,
      Color? color = null) {

    IClickableMenu.drawTextureBox(
        sb, texture: Game1.mouseCursors, sourceRect,
        (int) position.X, (int) position.Y, width ?? this.Width, height ?? this.Height,
        color ?? Color.White, scale: PixelZoom, drawShadow: false);
  }
}
