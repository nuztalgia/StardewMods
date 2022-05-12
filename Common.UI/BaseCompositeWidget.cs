using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class BaseWidget {

  internal abstract class Composite : BaseWidget {

    protected delegate void AdjustPosition(ref Vector2 position, int widgetWidth, int widgetHeight);

    private readonly record struct SubWidget(
        BaseWidget Widget,
        AdjustPosition? PreDraw,
        AdjustPosition? PostDraw
    );

    private readonly List<SubWidget> SubWidgets = new();

    protected void AddSubWidget(
        BaseWidget widget, AdjustPosition? preDraw = null, AdjustPosition? postDraw = null) {
      this.SubWidgets.Add(new SubWidget(widget, preDraw, postDraw));
    }

    protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
      int maxHeight = DefaultHeight;
      this.ForEachWidget((BaseWidget widget) => {
        (widget.Width, widget.Height) = widget.UpdateDimensions(totalWidth);
        maxHeight = Math.Max(widget.Height, maxHeight);
      });
      return (totalWidth, maxHeight);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      foreach (var (widget, preDraw, postDraw) in this.SubWidgets) {
        preDraw?.Invoke(ref position, widget.Width, widget.Height);
        widget.InternalDraw(sb, position);
        postDraw?.Invoke(ref position, widget.Width, widget.Height);
      }
    }

    protected override sealed void ResetState() {
      this.ForEachWidget((BaseWidget widget) => widget.ResetState());
    }

    protected override sealed void SaveState() {
      this.ForEachWidget((BaseWidget widget) => widget.SaveState());
    }

    private void ForEachWidget(Action<BaseWidget> action) {
      this.SubWidgets.ForEach((SubWidget subWidget) => action(subWidget.Widget));
    }
  }
}
