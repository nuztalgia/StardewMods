namespace Nuztalgia.StardewMods.Common.UI;

internal class DynamicText : SpriteFontWidget {

  protected override string RawText => this.GetText();

  private readonly Func<string> GetText;

  protected DynamicText(
      Func<string> getText, Font font, bool drawShadow, bool wrapLines, Alignment? alignment)
          : base(font, drawShadow, wrapLines, alignment) {
    this.GetText = getText;
  }

  internal static DynamicText CreateDropdownEntry(Func<string> getText) {
    return new(getText, Font.Small, drawShadow: false, wrapLines: false, alignment: null);
  }

  internal static DynamicText CreateSliderLabel(Func<string> getText) {
    return new(getText, Font.Regular, drawShadow: true, wrapLines: false, alignment: null);
  }
}
