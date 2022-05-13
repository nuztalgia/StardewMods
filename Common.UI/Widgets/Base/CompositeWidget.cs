using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal abstract class Composite : Widget {

    protected delegate void AdjustPosition(ref Vector2 position, int widgetWidth, int widgetHeight);

    private readonly record struct SubWidget(
        Widget Widget,
        AdjustPosition? PreDraw,
        AdjustPosition? PostDraw
    );

    private readonly List<SubWidget> SubWidgets = new();

    protected Composite(
        string? name = null, string? tooltip = null, Alignment? alignment = null)
            : base(name, tooltip, alignment: alignment) { }

    protected void AddSubWidget(
        Widget widget, AdjustPosition? preDraw = null, AdjustPosition? postDraw = null) {
      this.SubWidgets.Add(new SubWidget(widget, preDraw, postDraw));
    }

    protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
      (int maxWidth, int maxHeight) = (0, 0);
      this.ForEachWidget((Widget widget) => {
        (widget.Width, widget.Height) = widget.UpdateDimensions(totalWidth);
        maxWidth = Math.Max(widget.Width, maxWidth);
        maxHeight = Math.Max(widget.Height, maxHeight);
      });
      return (maxWidth, maxHeight);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      foreach (var (widget, preDraw, postDraw) in this.SubWidgets) {
        preDraw?.Invoke(ref position, widget.Width, widget.Height);
        widget.Draw(sb, position, this.Width, this.Height);
        postDraw?.Invoke(ref position, widget.Width, widget.Height);
      }
    }

    protected override sealed void ResetState() {
      this.ForEachWidget((Widget widget) => widget.ResetState());
    }

    protected override sealed void SaveState() {
      this.ForEachWidget((Widget widget) => widget.SaveState());
    }

    private void ForEachWidget(Action<Widget> action) {
      this.SubWidgets.ForEach((SubWidget subWidget) => action(subWidget.Widget));
    }
  }
}
