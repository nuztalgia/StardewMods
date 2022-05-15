using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal abstract class Composite : Widget {

    protected enum LinearMode {
      Off, Horizontal, Vertical
    }

    protected delegate void AdjustPosition(ref Vector2 position, int widgetWidth, int widgetHeight);

    private readonly record struct SubWidget(
        Widget Widget,
        AdjustPosition? PreDraw,
        AdjustPosition? PostDraw
    );

    private readonly List<SubWidget> SubWidgets = new();
    private readonly LinearMode Mode;

    protected Composite(
        string? name = null,
        string? tooltip = null,
        Alignment? alignment = null,
        LinearMode linearMode = LinearMode.Off)
            : base(name, tooltip, alignment) {
      this.Mode = linearMode;
    }

    protected Composite(Alignment? alignment, LinearMode linearMode)
        : this(name: null, tooltip: null, alignment, linearMode) { }

    protected Composite(Alignment? alignment) : this(name: null, tooltip: null, alignment) { }

    protected void AddSubWidget(
        Widget widget, AdjustPosition? preDraw = null, AdjustPosition? postDraw = null) {
      this.SubWidgets.Add(new SubWidget(widget, preDraw, postDraw));
    }

    protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
      (int width, int height) = (0, 0);
      this.ForEachWidget((Widget widget) => {
        (widget.Width, widget.Height) = widget.UpdateDimensions(totalWidth);

        width = (this.Mode == LinearMode.Horizontal)
            ? width + widget.Width
            : Math.Max(width, widget.Width);

        height = (this.Mode == LinearMode.Vertical)
            ? height + widget.Height
            : Math.Max(height, widget.Height);
      });
      return (width, height);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      foreach (var (widget, preDraw, postDraw) in this.SubWidgets) {
        preDraw?.Invoke(ref position, widget.Width, widget.Height);
        widget.Draw(sb, position, this.Width, this.Height);
        postDraw?.Invoke(ref position, widget.Width, widget.Height);

        if (this.Mode == LinearMode.Horizontal) {
          position.X += widget.Width;
        } else if (this.Mode == LinearMode.Vertical) {
          position.Y += widget.Height;
        }
      }
    }

    protected override sealed void ResetState() {
      this.ForEachWidget((Widget widget) => widget.ResetState());
    }

    protected override sealed void SaveState() {
      this.ForEachWidget((Widget widget) => widget.SaveState());
    }

    protected static (int width, int height) GetTextDimensions(TextWidget textWidget) {
      return textWidget.UpdateDimensions(int.MaxValue);
    }

    private void ForEachWidget(Action<Widget> action) {
      this.SubWidgets.ForEach((SubWidget subWidget) => action(subWidget.Widget));
    }
  }
}
