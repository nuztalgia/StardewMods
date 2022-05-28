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
    private readonly HashSet<Widget> HiddenWidgets = new();

    private readonly IDictionary<Widget, Func<bool>>? HideableWidgets;
    private readonly LinearMode Mode;
    private readonly bool IsFullWidth;
    private readonly bool IsHeightManager;

    protected int SubWidgetCount => this.SubWidgets.Count;
    protected int CompositeWidth => this.Width;
    protected int CompositeHeight => this.Height;

    private bool ShouldUpdateDimensionsOnDraw = false;

    protected Composite(
        string? name = null,
        string? tooltip = null,
        Alignment? alignment = null,
        IDictionary<Widget, Func<bool>>? hideableWidgets = null,
        LinearMode linearMode = LinearMode.Off,
        bool isFullWidth = false,
        bool isHeightManager = false)
            : base(name, tooltip, alignment) {

      this.HideableWidgets = hideableWidgets;
      this.Mode = linearMode;
      this.IsFullWidth = isFullWidth;
      this.IsHeightManager = isHeightManager;
    }

    protected void AddSubWidget(
        Widget widget, Adjustment? preDraw = null, Adjustment? postDraw = null) {
      this.SubWidgets.Add(new SubWidget(widget, preDraw, postDraw));
    }

    protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
      (int width, int height) = (0, 0);
      int? managedHeight = null;

      foreach ((Widget widget, Adjustment? preDraw, Adjustment? postDraw) in this.SubWidgets) {
        (widget.Width, widget.Height) = widget.UpdateDimensions(totalWidth);

        if (widget.NameLabel is Widget label) {
          (label.Width, label.Height) = label.UpdateDimensions(totalWidth);
          widget.Height = Math.Max(widget.Height, label.Height);
        }

        if (this.IsHeightManager && (this.Mode == LinearMode.Vertical)) {
          Vector2 measurement = Vector2.Zero;
          preDraw?.Invoke(ref measurement, widget.Width, widget.Height);
          postDraw?.Invoke(ref measurement, widget.Width, widget.Height);

          managedHeight = this.ShouldDrawWidget(widget)
              ? (height + widget.Height + (int) measurement.Y)
              : 0;
        }

        width = (this.Mode == LinearMode.Horizontal)
            ? width + widget.Width
            : Math.Max(width, widget.Width);

        height = (this.Mode == LinearMode.Vertical)
            ? managedHeight ?? (height + widget.Height)
            : Math.Max(height, widget.Height);
      }

      return (this is IOverlayable)
          ? IOverlayable.Dimensions
          : (this.IsFullWidth ? totalWidth : Math.Min(width, totalWidth), height);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      if (this.ShouldUpdateDimensionsOnDraw) {
        (this.Width, this.Height) = this.UpdateDimensions(ContainerWidth);
        this.ShouldUpdateDimensionsOnDraw = false;
      }

      foreach ((Widget widget, Adjustment? preDraw, Adjustment? postDraw) in this.SubWidgets) {
        if (widget == ActiveOverlay) {
          ActiveOverlayDrawPosition = new(position.X, position.Y);
        } else if (this.ShouldDrawWidget(widget)) {

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
    }

    protected override void ResetState() {
      foreach (SubWidget subWidget in this.SubWidgets) {
        subWidget.Widget.ResetState();
      }
    }

    protected override void SaveState() {
      foreach (SubWidget subWidget in this.SubWidgets) {
        subWidget.Widget.SaveState();
      }
    }

    protected static (int width, int height) GetTextDimensions(TextWidget textWidget) {
      return textWidget.UpdateDimensions(int.MaxValue);
    }

    private bool ShouldDrawWidget(Widget widget) {
      if ((this.HideableWidgets == null)
          || !this.HideableWidgets.TryGetValue(widget, out Func<bool>? hideWhen)) {
        return true;
      }

      bool shouldHide = hideWhen();

      if (this.IsHeightManager && (shouldHide != this.HiddenWidgets.Contains(widget))) {
        this.ShouldUpdateDimensionsOnDraw = true;
        if (!this.HiddenWidgets.Add(widget)) {
          this.HiddenWidgets.Remove(widget);
        }
      }
      return !shouldHide;
    }
  }
}
