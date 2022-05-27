namespace Nuztalgia.StardewMods.Common.UI;

internal class Label : StaticText {

  private class WithTooltip : Composite, IHoverable {

    private const int TooltipWidth = 800;

    private readonly Tooltip Tooltip;

    public bool IsHovering { get; set; }
    public string HoverSoundName => "shiny4";

    internal WithTooltip(string labelText, string tooltipText, string? tooltipTitle)
        : base(alignment: Alignment.Left) {

      this.Tooltip = new(
          text: ParseText(tooltipText, Font.Small, TooltipWidth),
          title: ParseText(tooltipTitle ?? labelText, Font.Regular, TooltipWidth));

      this.AddSubWidget(new Label(labelText));
      this.AddSubWidget(this.Tooltip);
    }

    public void OnHoverStart() {
      this.Tooltip.SetHoverState(isHovering: true);
    }

    public void OnHoverStop() {
      this.Tooltip.SetHoverState(isHovering: false);
    }
  }

  private Label(string text, Alignment? alignment = null)
      : base(text, Font.Regular, drawShadow: true, wrapLines: false, alignment) { }

  internal static Widget? Create(
      string? labelText,
      string? tooltipText,
      string? tooltipTitle = null) {

    return (labelText == null)
        ? null
        : (tooltipText == null)
            ? new Label(labelText, Alignment.Left)
            : new Label.WithTooltip(labelText, tooltipText, tooltipTitle);
  }
}
