using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Button : Widget.Composite {

  private class Background : Widget {

    private static readonly Rectangle SourceRect = new(432, 439, 9, 9);

    private Color TintColor => this.IsHovering ? Color.Wheat : Color.White;

    private readonly int TextWidth;
    private readonly int TextHeight;
    private readonly int TargetWidth;
    private readonly int TargetHeight;

    internal Vector2 TextOffset;

    internal Background(
        Action clickAction, int textWidth, int textHeight, int targetWidth, int targetHeight)
            : base(interaction: new Interaction.Clickable(clickAction)) {
      this.TextWidth = textWidth;
      this.TextHeight = textHeight;
      this.TargetWidth = targetWidth;
      this.TargetHeight = targetHeight;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      (int width, int height) = (Math.Min(totalWidth, this.TargetWidth), this.TargetHeight);
      this.TextOffset = new Vector2(width - this.TextWidth, height - this.TextHeight) / 2;
      return (width, height);
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
      Alignment alignment = Alignment.None,
      int? minWidth = null,
      int? minHeight = null,
      int? maxWidth = null,
      int? maxHeight = null) : base(alignment: alignment) {

    StaticText label = StaticText.CreateButtonLabel(labelText);
    Vector2 measuredText = label.MeasureSingleLine(labelText);
    (int textWidth, int textHeight) = ((int) measuredText.X, (int) measuredText.Y);

    Background background = new(
        clickAction: clickAction,
        textWidth: textWidth,
        textHeight: textHeight,
        targetWidth: Math.Clamp(textWidth + PaddingX, minWidth ?? 0, maxWidth ?? int.MaxValue),
        targetHeight: Math.Clamp(textHeight + PaddingY, minHeight ?? 0, maxHeight ?? int.MaxValue));

    this.AddSubWidget(background,
        postDraw: (ref Vector2 position, int _, int _) => position += background.TextOffset);
    this.AddSubWidget(label);
  }
}
