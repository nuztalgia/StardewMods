using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class ImageGroup : Widget.Composite {

  private readonly Action? ResetAction;
  private readonly Action? SaveAction;
  private readonly Spacing InnerPadding;

  private readonly AdjustPosition? InitialPreDraw;
  private readonly AdjustPosition? RegularPreDraw;
  private readonly AdjustPosition? RegularPostDraw;

  private ImageGroup(
      string? name,
      string? tooltip,
      Action? resetAction,
      Action? saveAction,
      Alignment? alignment,
      LinearMode linearMode,
      Spacing innerPadding,
      bool reverseBaseline)
          : base(name, tooltip, alignment, linearMode) {

    this.ResetAction = resetAction;
    this.SaveAction = saveAction;
    this.InnerPadding = innerPadding;

    if (reverseBaseline) {
      if (linearMode == LinearMode.Horizontal) {
        this.InitialPreDraw = (ref Vector2 position, int _, int height) =>
            position.Y += this.CompositeHeight - height;
        this.RegularPreDraw = (ref Vector2 position, int _, int height) => position.Y -= height;
        this.RegularPostDraw = (ref Vector2 position, int _, int height) => position.Y += height;
      }
      if (linearMode == LinearMode.Vertical) {
        this.InitialPreDraw = (ref Vector2 position, int width, int _) =>
            position.X += this.CompositeWidth - width;
        this.RegularPreDraw = (ref Vector2 position, int width, int _) => position.X -= width;
        this.RegularPostDraw = (ref Vector2 position, int width, int _) => position.X += width;
      }
    }
  }

  internal static ImageGroup CreateHorizontal(
      string? name = null,
      string? tooltip = null,
      Action? resetAction = null,
      Action? saveAction = null,
      int? innerPadding = null,
      Alignment? alignment = null,
      bool bottomAlign = false) {

    return new ImageGroup(
        name, tooltip, resetAction, saveAction, alignment, LinearMode.Horizontal,
        Spacing.CreateHorizontal(innerPadding), reverseBaseline: bottomAlign);
  }

  internal static ImageGroup CreateVertical(
      string? name = null,
      string? tooltip = null,
      Action? resetAction = null,
      Action? saveAction = null,
      int? innerPadding = null,
      Alignment? alignment = null,
      bool rightAlign = false) {

    return new ImageGroup(
        name, tooltip, resetAction, saveAction, alignment, LinearMode.Vertical,
        Spacing.CreateVertical(innerPadding), reverseBaseline: rightAlign);
  }

  internal void AddImage(
      Func<Texture2D[]> getSourceImages,
      Func<Rectangle[]>? getSourceRects = null,
      Func<Rectangle[]>? getDestRects = null,
      int? scale = null,
      int? fixedWidth = null,
      int? fixedHeight = null) {

    this.AddSubWidgetAndSpacing(
        new Image(getSourceImages, getSourceRects, getDestRects, scale, fixedWidth, fixedHeight));
  }

  protected override void ResetState() {
    this.ResetAction?.Invoke();
  }

  protected override void SaveState() {
    this.SaveAction?.Invoke();
  }

  private void AddSubWidgetAndSpacing(Widget widget) {
    if (this.SubWidgetCount == 0) {
      this.AddSubWidget(widget, preDraw: this.InitialPreDraw, postDraw: this.RegularPostDraw);
    } else {
      this.AddSubWidget(this.InnerPadding);
      this.AddSubWidget(widget, preDraw: this.RegularPreDraw, postDraw: this.RegularPostDraw);
    }
  }
}
