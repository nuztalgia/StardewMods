using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Header : BaseTextWidget {

  internal class WithButton : Composite {
    internal WithButton(string headerText, string buttonText, Action buttonAction) {
      this.AddSubWidget(new Header(headerText),
          postDraw: (ref Vector2 position, int _, int _) => position.Y -= 4);
      this.AddSubWidget(new Button(buttonText, buttonAction, Alignment.Right));
    }
  }

  protected override string Text { get; }
  protected override int SingleLineWidth => int.MaxValue; // Always fill the available width.
  protected override int SingleLineHeight { get; }

  internal Header(string text) : base(alignment: Alignment.Left, wrapLines: false) {
    this.Text = text;
    this.SingleLineHeight = SpriteText.getHeightOfString(text);
  }

  internal override Vector2 MeasureSingleLine(string text) {
    return new(SpriteText.getWidthOfString(text), SpriteText.getHeightOfString(text));
  }

  protected override void Draw(SpriteBatch sb, Vector2 position, string text) {
    SpriteText.drawString(sb, text, (int) position.X, (int) position.Y);
  }
}
