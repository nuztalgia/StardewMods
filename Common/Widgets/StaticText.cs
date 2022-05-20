namespace Nuztalgia.StardewMods.Common.UI;

internal class StaticText : SpriteFontWidget {

  private sealed class WidgetLabel : StaticText, IHoverable {

    private const int TooltipWidth = 800;

    private readonly string TooltipText;
    private readonly string TooltipTitle;

    public bool IsHovering { get; set; }

    internal WidgetLabel(string labelText, string tooltipText, string tooltipTitle)
        : base(labelText, Font.Regular, drawShadow: true, wrapLines: false, Alignment.Left) {

      this.TooltipText = ParseText(tooltipText, Font.Small, TooltipWidth);
      this.TooltipTitle = ParseText(tooltipTitle, Font.Regular, TooltipWidth);
    }

    protected override void PostDraw(SpriteBatch sb) {
      if (this.IsHovering) {
        DrawTooltip(sb, this.TooltipText, this.TooltipTitle);
      }
    }
  }

  protected override string RawText { get; }

  protected StaticText(
      string text, Font font, bool drawShadow, bool wrapLines, Alignment? alignment)
          : base(font, drawShadow, wrapLines, alignment) {
    this.RawText = text;
  }

  internal static StaticText CreateButtonLabel(string text) {
    return new(text, Font.Small, drawShadow: false, wrapLines: false, Alignment.CenterXY);
  }

  internal static StaticText CreateImageCaption(string text) {
    return new(text, Font.Regular, drawShadow: true, wrapLines: false, Alignment.CenterX);
  }

  internal static StaticText CreateParagraph(string text) {
    return new(text, Font.Small, drawShadow: false, wrapLines: true, Alignment.Left);
  }

  internal static StaticText CreateWidgetLabel(
      string labelText, string tooltipText, string? tooltipTitle = null) {
    return new WidgetLabel(labelText, tooltipText, tooltipTitle ?? labelText);
  }
}
