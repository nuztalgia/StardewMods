using System;

namespace Nuztalgia.StardewMods.Common.UI;

internal sealed class DynamicText : BaseTextWidget.Simple {

  protected override string Text => this.GetText();

  private readonly Func<string> GetText;

  private DynamicText(Func<string> getText, FontSize fontSize, bool drawShadow, bool wrapLines)
      : base(fontSize, drawShadow, wrapLines) {
    this.GetText = getText;
  }

  internal static DynamicText CreateOptionLabel(Func<string> getText) {
    return new(getText, FontSize.Regular, drawShadow: true, wrapLines: false);
  }
}
