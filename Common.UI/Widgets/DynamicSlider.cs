using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nuztalgia.StardewMods.Common.UI;

internal class DynamicSlider : BaseWidgetWithValue<int>, Extensions.IDraggable {

  private const int RawBarWidth = 10;
  private const int RawBarHeight = 6;

  private const int ScaledBarWidth = RawBarWidth * PixelZoom;
  private const int ScaledBarHeight = RawBarHeight * PixelZoom;
  private const int ScaledPadding = ScaledBarHeight / 2;

  private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawBarHeight);
  private static readonly Rectangle TrackSourceRect = new(403, 383, 6, RawBarHeight);

  private static int TrackWidth;
  private static int TextOffset;

  public bool IsDragging { get; set; }
  public ButtonState MouseButtonState { get; set; }
  public (int X, int Y) MousePosition { get; set; }

  private readonly Func<int>? GetMinValue;
  private readonly Func<int>? GetMaxValue;

  private int MinValue;
  private int MaxValue;

  internal DynamicSlider(
      string name,
      Func<int> getValue,
      Action<int> saveValue,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? valueToString = null,
      Action<int>? onValueChange = null,
      string? tooltip = null)
          : base(name, getValue, saveValue, valueToString, onValueChange, tooltip) {
    this.GetMinValue = getDynamicMinValue;
    this.GetMaxValue = getDynamicMaxValue;
    this.MinValue = staticMinValue ?? int.MinValue;
    this.MaxValue = staticMaxValue ?? int.MaxValue;
  }

  protected override void UpdateState(Vector2 position) {
    this.UpdateMouseState(position, customDraggableWidth: TrackWidth);

    if (this.GetMinValue is not null) {
      this.MinValue = this.GetMinValue();
    }
    if (this.GetMaxValue is not null) {
      this.MaxValue = this.GetMaxValue();
    }

    this.Value = Math.Clamp(
        this.IsDragging ? GetValueByMousePosition() : this.Value,
        this.MinValue,
        this.MaxValue);

    int GetValueByMousePosition() {
      float mousePositionPercent = (this.MousePosition.X - position.X) / TrackWidth;
      return (int) (mousePositionPercent * (this.MaxValue - this.MinValue)) + this.MinValue;
    }
  }

  protected override void DrawState(SpriteBatch sb, Vector2 position) {
    sb.DrawString(MainFont, this.ValueText, new(position.X + TextOffset, position.Y));

    position.Y += ScaledPadding;
    sb.DrawTextureBox(CursorsImage, position, TrackSourceRect, TrackWidth, ScaledBarHeight);

    float valuePercent = (this.Value - this.MinValue) / (float) (this.MaxValue - this.MinValue);
    position.X += valuePercent * (TrackWidth - ScaledBarWidth);
    sb.Draw(CursorsImage, position, BarSourceRect);
  }

  protected override (int width, int height) CalculateDimensions(int totalWidth) {
    TrackWidth = totalWidth / 3;
    TextOffset = TrackWidth + (ScaledPadding * 2);
    return (totalWidth / 2, DefaultHeight);
  }
}
