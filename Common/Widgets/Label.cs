namespace Nuztalgia.StardewMods.Common.UI;

internal class Label : StaticText {

  private class Tooltip : Widget {

    private readonly string TooltipText;
    private readonly string TooltipTitle;

    internal Tooltip(string tooltipText, string tooltipTitle) {
      this.TooltipText = tooltipText;
      this.TooltipTitle = tooltipTitle;
    }

    internal void SetHoverState(bool isHovering) {
      this.SetOverlayStatus(isActive: isHovering);
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      return (0, 0); // Tooltip does not occupy any "real estate" on the page.
    }

    protected override void Draw(SpriteBatch sb, Vector2 position) {
      if (this == ActiveOverlay) {
        DrawTooltip(sb, this.TooltipText, this.TooltipTitle);
      }
    }
  }

  private class WithTooltip : Composite, IHoverable {

    private const int TooltipWidth = 800;

    private readonly Tooltip Tooltip;

    public bool IsHovering {
      get => this.IsHovering_Field;
      set {
        this.Tooltip.SetHoverState(value);
        this.IsHovering_Field = value;
      }
    }

    private bool IsHovering_Field = false;

    internal WithTooltip(string labelText, string tooltipText, string? tooltipTitle)
        : base(Alignment.Left) {

      this.Tooltip = new(
          tooltipText: ParseText(tooltipText, Font.Small, TooltipWidth),
          tooltipTitle: ParseText(tooltipTitle ?? labelText, Font.Regular, TooltipWidth));

      this.AddSubWidget(new Label(labelText));
      this.AddSubWidget(this.Tooltip);
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
