namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class SpriteFontWidget : TextWidget {

  protected enum Font { Regular, Small }

  protected override int LineHeight { get; }
  protected override MeasureWidth MeasureTextWidth { get; }

  private readonly SpriteFont SpriteFont;
  private readonly bool DrawShadow;

  protected SpriteFontWidget(Font font, bool drawShadow, bool wrapLines, Alignment? alignment)
      : base(wrapLines, alignment) {

    this.SpriteFont = (font == Font.Small) ? Game1.smallFont : Game1.dialogueFont;
    this.DrawShadow = drawShadow;

    // This type of widget's LineHeight is purely based on the Font that was specified.
    this.LineHeight = (int) this.SpriteFont.MeasureString("This text is irrelevant!").Y;
    this.MeasureTextWidth = (string text) => (int) this.SpriteFont.MeasureString(text).X;
  }

  protected override sealed void Draw(SpriteBatch sb, Vector2 position, string text) {
    if (this.DrawShadow) {
      Utility.drawTextWithShadow(sb, text, this.SpriteFont, position, Game1.textColor);
    } else {
      sb.DrawString(
          this.SpriteFont, text, position, Game1.textColor,
          rotation: 0f, origin: Vector2.Zero, scale: 1f,
          effects: SpriteEffects.None, layerDepth: 1f);
    }
  }
}
