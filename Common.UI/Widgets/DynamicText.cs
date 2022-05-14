using System;

namespace Nuztalgia.StardewMods.Common.UI;

internal class DynamicText : SpriteFontWidget {

  protected override string RawText => this.GetText();

  private readonly Func<string> GetText;

  protected DynamicText(
      Func<string> getText, Font font, bool drawShadow, bool wrapLines, Alignment? alignment = null)
          : base(font, drawShadow, wrapLines, alignment) {
    this.GetText = getText;
  }

  internal static DynamicText CreateOptionLabel(Func<string> getText) {
    return new(getText, Font.Regular, drawShadow: true, wrapLines: false);
  }
}
