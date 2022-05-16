using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Header : TextWidget {

  internal class WithButton : Composite {
    internal WithButton(string headerText, string buttonText, Action buttonAction) {
      this.AddSubWidget(new Header(headerText),
          postDraw: (ref Vector2 position, int _, int _) => position.Y -= PixelZoom);
      this.AddSubWidget(new Button(buttonText, buttonAction, alignment: Alignment.Right));
    }
  }

  protected override string RawText { get; }
  protected override int LineHeight { get; }
  protected override MeasureWidth MeasureTextWidth { get; }

  internal Header(string text) : base(alignment: Alignment.Left, wrapLines: false) {
    this.RawText = text;
    this.LineHeight = SpriteText.getHeightOfString(text);
    this.MeasureTextWidth = (string text) => SpriteText.getWidthOfString(text);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position, string text) {
    SpriteText.drawString(sb, text, (int) position.X, (int) position.Y);
  }
}
