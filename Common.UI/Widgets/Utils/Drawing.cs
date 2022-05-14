using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  protected const int PixelZoom = Game1.pixelZoom;

  protected static void DrawFromCursors(SpriteBatch sb, Vector2 position, Rectangle sourceRect) {
    sb.Draw(
        texture: Game1.mouseCursors, position, sourceRect, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale: PixelZoom, effects: SpriteEffects.None, layerDepth: 1f);
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
