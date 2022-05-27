namespace Nuztalgia.StardewMods.Common.UI;

internal class Button : Widget.Composite {

  private class Background : Widget, IClickable, IHoverable {

    private static readonly Rectangle SourceRect = new(432, 439, 9, 9);

    public Action ClickAction { get; }
    public string ClickSoundName => "bigSelect";

    public bool IsHovering { get; set; }
    public string HoverSoundName => "Cowboy_Footstep";

    private Color TintColor => this.IsHovering ? Color.Wheat : Color.White;

    private readonly int TargetWidth;
    private readonly int TargetHeight;

    internal Background(Action clickAction, int targetWidth, int targetHeight) {
      this.ClickAction = clickAction;
      this.TargetWidth = targetWidth;
      this.TargetHeight = targetHeight;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      return (Math.Min(totalWidth, this.TargetWidth), this.TargetHeight);
    }

    protected override void Draw(SpriteBatch sb, Vector2 position) {
      this.DrawFromCursors(sb, position, SourceRect, color: this.TintColor);
    }
  }

  private const int PaddingX = 32;
  private const int PaddingY = 16;

  internal Button(
      string labelText,
      Action clickAction,
      int? minWidth = null,
      int? minHeight = null,
      int? maxWidth = null,
      int? maxHeight = null,
      Alignment? alignment = null)
          : base(alignment: alignment) {

    StaticText label = StaticText.CreateButtonLabel(labelText);
    (int textWidth, int textHeight) = GetTextDimensions(label);

    Background background = new(
        clickAction: clickAction,
        targetWidth: Math.Clamp(textWidth + PaddingX, minWidth ?? 0, maxWidth ?? int.MaxValue),
        targetHeight: Math.Clamp(textHeight + PaddingY, minHeight ?? 0, maxHeight ?? int.MaxValue));

    this.AddSubWidget(background);
    this.AddSubWidget(label);
  }
}
