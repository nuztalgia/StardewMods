namespace Nuztalgia.StardewMods.Common.UI;

internal class StaticText : BaseTextWidget.Simple {

  protected override string Text { get; }

  protected StaticText(
      string text,
      FontSize fontSize,
      Alignment alignment,
      bool wrapLines,
      bool drawShadow)
          : base(fontSize, alignment, wrapLines, drawShadow) {
    this.Text = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, FontSize.Small, Alignment.None, wrapLines: false, drawShadow: false);
  }
}
