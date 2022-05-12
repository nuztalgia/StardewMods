namespace Nuztalgia.StardewMods.Common.UI;

internal class StaticText : BaseWidget.Text.SpriteFont {

  protected override string RawText { get; }

  protected StaticText(
      string text,
      FontSize fontSize,
      Alignment alignment,
      bool wrapLines,
      bool drawShadow)
          : base(fontSize, alignment, wrapLines, drawShadow) {
    this.RawText = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, FontSize.Small, Alignment.None, wrapLines: false, drawShadow: false);
  }
}
