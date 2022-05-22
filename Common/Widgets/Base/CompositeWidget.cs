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

    private readonly IList<SubWidget> SubWidgets = new List<SubWidget>();
    private readonly IDictionary<Widget, Func<bool>>? HideableWidgets;
    private readonly LinearMode Mode;
    private readonly bool IsFullWidth;
    private readonly bool CanDrawOverlay;

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

    protected Composite(IDictionary<Widget, Func<bool>>? hideableWidgets, bool canDrawOverlay)
        : this(linearMode: LinearMode.Vertical, isFullWidth: true) {
      // More "powerful" constructor, only used by MenuPage (for now).
      this.HideableWidgets = hideableWidgets;
      this.CanDrawOverlay = canDrawOverlay;
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

      foreach ((Widget widget, Adjustment? preDraw, Adjustment? postDraw) in this.SubWidgets) {
        if (widget == ActiveOverlay) {
          ActiveOverlayPosition = new(position.X, position.Y);
        } else if (!ShouldHideWidget(widget)) {
          DrawSubWidget(widget, preDraw, postDraw);
          AdjustPosition(widget);
        }
      }

      if (this.CanDrawOverlay && (ActiveOverlayPosition != default)) {
        ActiveOverlay?.Draw(sb, ActiveOverlayPosition);
      }

      bool ShouldHideWidget(Widget widget) {
        return (this.HideableWidgets != null)
            && this.HideableWidgets.TryGetValue(widget, out Func<bool>? shouldHide)
            && shouldHide();
      }

      void DrawSubWidget(Widget widget, Adjustment? preDraw, Adjustment? postDraw) {
        preDraw?.Invoke(ref position, widget.Width, widget.Height);
        widget.Draw(sb, position, this.Width, this.Height);
        widget.NameLabel?.Draw(sb, position, this.Width, this.Height);
        postDraw?.Invoke(ref position, widget.Width, widget.Height);
      }

      void AdjustPosition(Widget widget) {
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
