using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Slider : Widget.Composite {

  private class TrackBar : OptionWidget<int>, IDraggable {

    private const int RawBarWidth = 10;
    private const int RawBarHeight = 6;

    private const int ScaledBarWidth = RawBarWidth * PixelZoom;
    private const int ScaledBarHeight = RawBarHeight * PixelZoom;
    private const int ScaledPadding = (DefaultHeight - ScaledBarHeight) / 2;

    private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawBarHeight);
    private static readonly Rectangle TrackSourceRect = new(403, 383, 6, RawBarHeight);

    private static int TrackWidth;

    public bool IsDragging { get; set; }

    private readonly Func<int> GetMinValue;
    private readonly Func<int> GetMaxValue;

    internal TrackBar(
        Func<int> loadValue,
        Action<int> saveValue,
        Action<int>? onValueChanged = null,
        int? staticMinValue = null,
        int? staticMaxValue = null,
        Func<int>? getDynamicMinValue = null,
        Func<int>? getDynamicMaxValue = null)
            : base(loadValue, saveValue, onValueChanged) {

      this.GetMinValue = getDynamicMinValue ?? (() => staticMinValue ?? int.MinValue);
      this.GetMaxValue = getDynamicMaxValue ?? (() => staticMaxValue ?? int.MaxValue);
    }

    internal int GetValue() {
      return this.Value;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      TrackWidth = totalWidth / 3;
      return (TrackWidth, DefaultHeight);
    }

    protected override void Draw(SpriteBatch sb, Vector2 position) {
      (int min, int max) = (this.GetMinValue(), this.GetMaxValue());
      (min, max) = (Math.Min(min, max), Math.Max(min, max));
      float valueRange = max - min;

      if (this.IsDragging) {
        float mouseValue = (MousePositionX - position.X) / TrackWidth * valueRange;
        this.Value = Math.Clamp((int) mouseValue + min, min, max);
      }

      position.Y += ScaledPadding;
      this.DrawFromCursors(sb, position, TrackSourceRect, height: ScaledBarHeight);

      float valuePercent = (this.Value - min) / valueRange;
      position.X += valuePercent * (TrackWidth - ScaledBarWidth);
      DrawFromCursors(sb, position, BarSourceRect);
    }
  }

  internal Slider(
      string name,
      Func<int> loadValue,
      Action<int> saveValue,
      Action<int>? onValueChanged = null,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? formatValue = null,
      string? tooltip = null) : base(name, tooltip, LinearMode.Horizontal) {

    TrackBar trackBar = new(
        loadValue, saveValue, onValueChanged,
        staticMinValue, staticMaxValue,
        getDynamicMinValue, getDynamicMaxValue);

    Func<string> getValueText = (formatValue == null)
        ? () => trackBar.GetValue().ToString()
        : () => formatValue(trackBar.GetValue());

    this.AddSubWidget(trackBar);
    this.AddSubWidget(Spacing.DefaultHorizontal);
    this.AddSubWidget(DynamicText.CreateOptionLabel(getValueText));
  }
}
