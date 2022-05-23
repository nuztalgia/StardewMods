namespace Nuztalgia.StardewMods.Common.UI;

internal class StaticText : SpriteFontWidget {

  protected override string RawText { get; }

  protected StaticText(
      string text, Font font, bool drawShadow, bool wrapLines, Alignment? alignment)
          : base(font, drawShadow, wrapLines, alignment) {
    this.RawText = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, Font.Small, drawShadow: false, wrapLines: false, Alignment.CenterXY);
  }

  internal static StaticText CreateDropdownEntry(string text) {
    return new(text, Font.Small, drawShadow: false, wrapLines: false, alignment: null);
  }

  internal static StaticText CreateImageCaption(string text) {
    return new(text, Font.Regular, drawShadow: true, wrapLines: false, Alignment.CenterX);
  }

  internal static StaticText CreateParagraph(string text) {
    return new(text, Font.Small, drawShadow: false, wrapLines: true, Alignment.Left);
  }
}
