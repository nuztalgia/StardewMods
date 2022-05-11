using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class DynamicSlider : BaseWidget.Option<int> {

  private const int RawBarWidth = 10;
  private const int RawBarHeight = 6;

  private const int ScaledBarWidth = RawBarWidth * PixelZoom;
  private const int ScaledBarHeight = RawBarHeight * PixelZoom;
  private const int ScaledPadding = ScaledBarHeight / 2;

  private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawBarHeight);
  private static readonly Rectangle TrackSourceRect = new(403, 383, 6, RawBarHeight);

  private static int TrackWidth;
  private static int TextOffset;

  private readonly Func<int, string> ValueToString;

  private readonly Func<int>? GetMinValue;
  private readonly Func<int>? GetMaxValue;

  private int MinValue;
  private int MaxValue;

  internal DynamicSlider(
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

    this.ValueToString = valueToString ?? (value => value.ToString());
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

  protected override int UpdateValue(Vector2 position, int previousValue) {
    if (this.GetMinValue != null) {
      this.MinValue = this.GetMinValue();
    }

    if (this.GetMaxValue != null) {
      this.MaxValue = this.GetMaxValue();
    }

    return this.IsDragging
        ? Math.Clamp(GetValueByMousePosition(), this.MinValue, this.MaxValue)
        : previousValue;

    int GetValueByMousePosition() {
      float mousePositionPercent = (this.MousePosition.X - position.X) / TrackWidth;
      return (int) (mousePositionPercent * (this.MaxValue - this.MinValue)) + this.MinValue;
    }
  }

  protected override void Draw(SpriteBatch sb, Vector2 position, int currentValue) {
    sb.DrawString(
        MainFont, this.ValueToString(currentValue), new(position.X + TextOffset, position.Y));

    position.Y += ScaledPadding;
    sb.DrawTextureBox(Cursors, position, TrackSourceRect, TrackWidth, ScaledBarHeight);

    float valuePercent = (currentValue - this.MinValue) / (float) (this.MaxValue - this.MinValue);
    position.X += valuePercent * (TrackWidth - ScaledBarWidth);
    sb.Draw(Cursors, position, BarSourceRect);
  }
}
