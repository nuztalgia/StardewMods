using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Slider : BaseWidget.Option<int> {

  private const int RawBarWidth = 10;
  private const int RawBarHeight = 6;

  private const int ScaledBarWidth = RawBarWidth * PixelZoom;
  private const int ScaledBarHeight = RawBarHeight * PixelZoom;
  private const int ScaledPadding = ScaledBarHeight / 2;

  private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawBarHeight);
  private static readonly Rectangle TrackSourceRect = new(403, 383, 6, RawBarHeight);

  private static int TrackWidth;
  private static int TextOffset;

  private readonly DynamicText Label;

  private readonly Func<int>? GetMinValue;
  private readonly Func<int>? GetMaxValue;

  private int MinValue;
  private int MaxValue;

  internal Slider(
      string name,
      Func<int> loadValue,
      Action<int> saveValue,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Action<int>? onValueChanged = null,
      Func<int, string>? valueToString = null,
      string? tooltip = null)
          : base(name, loadValue, saveValue, onValueChanged, tooltip, new Interaction.Draggable()) {

    this.Label = DynamicText.CreateOptionLabel(
        (valueToString == null) ? () => this.Value.ToString() : () => valueToString(this.Value));

    this.GetMinValue = getDynamicMinValue;
    this.GetMaxValue = getDynamicMaxValue;
    this.MinValue = staticMinValue ?? int.MinValue;
    this.MaxValue = staticMaxValue ?? int.MaxValue;
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    TrackWidth = totalWidth / 3;
    TextOffset = TrackWidth + (ScaledPadding * 2);
    return (TrackWidth, DefaultHeight);
  }

  protected override int UpdateValue(Vector2 position) {
    if (this.GetMinValue != null) {
      this.MinValue = this.GetMinValue();
    }

    if (this.GetMaxValue != null) {
      this.MaxValue = this.GetMaxValue();
    }

    return this.IsDragging
        ? Math.Clamp(GetValueByMousePosition(), this.MinValue, this.MaxValue)
        : this.Value;

    int GetValueByMousePosition() {
      float mousePositionPercent = (this.MousePosition.X - position.X) / TrackWidth;
      return (int) (mousePositionPercent * (this.MaxValue - this.MinValue)) + this.MinValue;
    }
  }

  protected override void DrawOption(SpriteBatch sb, Vector2 position) {
    sb.DrawWidget(this.Label, new(position.X + TextOffset, position.Y));

    position.Y += ScaledPadding;
    sb.DrawFromCursors(position, TrackSourceRect, TrackWidth, ScaledBarHeight);

    float valuePercent = (this.Value - this.MinValue) / (float) (this.MaxValue - this.MinValue);
    position.X += valuePercent * (TrackWidth - ScaledBarWidth);
    sb.DrawFromCursors(position, BarSourceRect);
  }
}
