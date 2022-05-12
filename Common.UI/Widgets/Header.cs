using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Header : BaseTextWidget {

  protected override string Text { get; }
  protected override int SingleLineWidth => int.MaxValue; // Always fill the available width.
  protected override int SingleLineHeight { get; }

  internal Header(string text) {
    this.Text = text;
    this.SingleLineHeight = SpriteText.getHeightOfString(text);
  }

  internal override Vector2 MeasureSingleLine(string text) {
    return new(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text));
  }

  protected override void Draw(SpriteBatch sb, Vector2 position, string text) {
    position.X -= (this.Width / 2) + 8; // Align left.
    SpriteText.drawString(sb, text, (int) position.X, (int) position.Y);
  }
}
