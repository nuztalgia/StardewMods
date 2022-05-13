using System;

namespace Nuztalgia.StardewMods.Common.UI;

internal class DynamicText : Widget.Text.SpriteFont {

  protected override string RawText => this.GetText();

  private readonly Func<string> GetText;

  protected DynamicText(
      Func<string> getText,
      FontSize fontSize,
      Alignment? alignment,
      bool wrapLines,
      bool drawShadow)
          : base(fontSize, alignment, wrapLines, drawShadow) {
    this.GetText = getText;
  }

  internal static DynamicText CreateOptionLabel(Func<string> getText) {
    return new(getText, FontSize.Regular, alignment: null, wrapLines: false, drawShadow: true);
  }
}
