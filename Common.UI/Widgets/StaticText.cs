namespace Nuztalgia.StardewMods.Common.UI;

internal sealed class StaticText : BaseTextWidget.Simple {

  protected override string Text { get; }

  private StaticText(string text, FontSize fontSize, bool drawShadow, bool wrapLines)
      : base(fontSize, drawShadow, wrapLines) {
    this.Text = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, FontSize.Regular, drawShadow: false, wrapLines: false);
  }
}
