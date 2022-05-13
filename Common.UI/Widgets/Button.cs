using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Button : Widget.Composite {

  private class Background : Widget {

    private static readonly Rectangle SourceRect = new(432, 439, 9, 9);

    private Color TintColor => this.IsHovering ? Color.Wheat : Color.White;

    private readonly int TargetWidth;
    private readonly int TargetHeight;

    internal Background(Action clickAction, int targetWidth, int targetHeight)
        : base(interaction: new Interaction.Clickable(clickAction)) {
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
      Alignment? alignment = null,
      int? minWidth = null,
      int? minHeight = null,
      int? maxWidth = null,
      int? maxHeight = null) : base(alignment: alignment) {

    StaticText label = StaticText.CreateButtonLabel(labelText);
    Vector2 measuredText = label.MeasureSingleLine(labelText);
    (int textWidth, int textHeight) = ((int) measuredText.X, (int) measuredText.Y);

    Background background = new(
        clickAction: clickAction,
        targetWidth: Math.Clamp(textWidth + PaddingX, minWidth ?? 0, maxWidth ?? int.MaxValue),
        targetHeight: Math.Clamp(textHeight + PaddingY, minHeight ?? 0, maxHeight ?? int.MaxValue));

    this.AddSubWidget(background);
    this.AddSubWidget(label);
  }
}
