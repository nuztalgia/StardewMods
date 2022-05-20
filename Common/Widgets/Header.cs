using StardewValley.BellsAndWhistles;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Header : TextWidget {

  internal class WithButton : Composite {

    private static readonly Vector2 PositionDiff = new(LeftShift, PixelZoom);

    internal WithButton(string headerText, string buttonText, Action buttonAction)
        : base(isFullWidth: true) {
      this.AddSubWidget(new Header(headerText),
          postDraw: (ref Vector2 position, int _, int _) => position -= PositionDiff);
      this.AddSubWidget(new Button(buttonText, buttonAction, alignment: Alignment.Right));
    }
  }

  private const int LeftShift = 8;

  protected override string RawText { get; }
  protected override int LineHeight { get; }
  protected override MeasureWidth MeasureTextWidth { get; }

  internal Header(string text) : base(alignment: Alignment.Left, wrapLines: false) {
    this.RawText = text;
    this.LineHeight = SpriteText.getHeightOfString(text);
    this.MeasureTextWidth = (string text) => SpriteText.getWidthOfString(text);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position, string text) {
    SpriteText.drawString(sb, text, (int) position.X - LeftShift, (int) position.Y);
  }
}
