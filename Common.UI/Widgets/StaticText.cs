namespace Nuztalgia.StardewMods.Common.UI;

internal class StaticText : BaseTextWidget.Simple {

  protected override string Text { get; }

  protected StaticText(string text, FontSize fontSize, bool drawShadow, bool wrapLines)
      : base(fontSize, drawShadow, wrapLines) {
    this.Text = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, FontSize.Regular, drawShadow: false, wrapLines: false);
  }
}
