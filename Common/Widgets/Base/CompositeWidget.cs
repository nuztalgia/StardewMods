namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal abstract class Composite : Widget {

    protected enum LinearMode {
      Off, Horizontal, Vertical
    }

    protected delegate void Adjustment(ref Vector2 position, int widgetWidth, int widgetHeight);

    private readonly record struct SubWidget(
        Widget Widget,
        Adjustment? PreDraw,
        Adjustment? PostDraw
    );

    private readonly List<SubWidget> SubWidgets = new();
    private readonly LinearMode Mode;
    private readonly bool IsFullWidth;

    protected int SubWidgetCount => this.SubWidgets.Count;
    protected int CompositeWidth => this.Width;
    protected int CompositeHeight => this.Height;

    protected Composite(
        string? name = null,
        string? tooltip = null,
        Alignment? alignment = null,
        LinearMode linearMode = LinearMode.Off,
        bool isFullWidth = false)
            : base(name, tooltip, alignment) {
      this.Mode = linearMode;
      this.IsFullWidth = isFullWidth;
    }

    protected Composite(string? name, string? tooltip, LinearMode linearMode)
        : this(name, tooltip, alignment: null, linearMode) { }

    protected Composite(Alignment? alignment, LinearMode linearMode = LinearMode.Off)
        : this(name: null, tooltip: null, alignment, linearMode) { }

    protected void AddSubWidget(
        Widget widget, Adjustment? preDraw = null, Adjustment? postDraw = null) {
      this.SubWidgets.Add(new SubWidget(widget, preDraw, postDraw));
    }

    protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
      (int width, int height) = (0, 0);
      this.ForEachWidget((Widget widget) => {
        (widget.Width, widget.Height) = widget.UpdateDimensions(totalWidth);

        if (widget.NameLabel is Widget label) {
          (label.Width, label.Height) = label.UpdateDimensions(totalWidth);
          widget.Height = Math.Max(widget.Height, label.Height);
        }

        widget.Height = Math.Max(widget.Height, DefaultHeight);

        width = (this.Mode == LinearMode.Horizontal)
            ? width + widget.Width
            : Math.Max(width, widget.Width);

        height = (this.Mode == LinearMode.Vertical)
            ? height + widget.Height
            : Math.Max(height, widget.Height);
      });
      return (this.IsFullWidth ? totalWidth : Math.Min(width, totalWidth), height);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      Vector2? overlayPosition = null;

      foreach ((Widget widget, Adjustment? preDraw, Adjustment? postDraw) in this.SubWidgets) {
        if (widget == ActiveOverlay) {
          overlayPosition = new(position.X, position.Y);
        } else {
          DrawSubWidget(widget, preDraw, postDraw);
        }
      }

      this.PostDraw(sb, overlayPosition ?? position);

      void DrawSubWidget(Widget widget, Adjustment? preDraw, Adjustment? postDraw) {
        preDraw?.Invoke(ref position, widget.Width, widget.Height);
        widget.Draw(sb, position, this.Width, this.Height);
        widget.NameLabel?.Draw(sb, position, this.Width, this.Height);
        postDraw?.Invoke(ref position, widget.Width, widget.Height);

        if (this.Mode == LinearMode.Horizontal) {
          position.X += widget.Width;
        } else if (this.Mode == LinearMode.Vertical) {
          position.Y += widget.Height;
        }
      }
    }

    protected override void ResetState() {
      this.ForEachWidget((Widget widget) => widget.ResetState());
    }

    protected override void SaveState() {
      this.ForEachWidget((Widget widget) => widget.SaveState());
    }

    protected virtual void PostDraw(SpriteBatch sb, Vector2 position) { }

    protected static (int width, int height) GetTextDimensions(TextWidget textWidget) {
      return textWidget.UpdateDimensions(int.MaxValue);
    }

    private void ForEachWidget(Action<Widget> action) {
      foreach (SubWidget subWidget in this.SubWidgets) {
        action(subWidget.Widget);
      }
    }
  }
}
