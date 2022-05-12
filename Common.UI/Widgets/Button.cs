using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Button : BaseWidget {

  private const int HorizontalPadding = 32;
  private const int VerticalPadding = 16;

  private static readonly Rectangle SourceRect = new(432, 439, 9, 9);

  private Color TintColor => this.IsHovering ? Color.Wheat : Color.White;

  private readonly StaticText Label;

  private readonly int TextWidth;
  private readonly int TextHeight;

  private readonly int TargetWidth;
  private readonly int TargetHeight;

  private Vector2 TextOffset;

  internal Button(
      string labelText,
      Action clickAction,
      Alignment alignment = Alignment.None,
      int? minWidth = null,
      int? minHeight = null,
      int? maxWidth = null,
      int? maxHeight = null)
          : base(interaction: new Interaction.Clickable(clickAction), alignment: alignment) {

    this.Label = StaticText.CreateButtonLabel(labelText);
    Vector2 measuredText = this.Label.MeasureSingleLine(labelText);
    (this.TextWidth, this.TextHeight) = ((int) measuredText.X, (int) measuredText.Y);

    this.TargetWidth =
        Math.Clamp(this.TextWidth + HorizontalPadding, minWidth ?? 0, maxWidth ?? int.MaxValue);
    this.TargetHeight =
        Math.Clamp(this.TextHeight + VerticalPadding, minHeight ?? 0, maxHeight ?? int.MaxValue);
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    (int width, int height) = (Math.Min(totalWidth, this.TargetWidth), this.TargetHeight);
    this.TextOffset = new Vector2(width - this.TextWidth, height - this.TextHeight) / 2;
    return (width, height);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    sb.DrawFromCursors(position, SourceRect, this.Width, this.Height, this.TintColor);
    position += this.TextOffset;
    sb.DrawWidget(this.Label, position);
  }
}
