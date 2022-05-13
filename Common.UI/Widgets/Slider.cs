using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Slider : Widget.Composite {

  private class TrackBar : Option<int> {

    private const int RawBarWidth = 10;
    private const int RawBarHeight = 6;

    private const int ScaledBarWidth = RawBarWidth * PixelZoom;
    private const int ScaledBarHeight = RawBarHeight * PixelZoom;
    private const int ScaledPadding = (DefaultHeight - ScaledBarHeight) / 2;

    private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawBarHeight);
    private static readonly Rectangle TrackSourceRect = new(403, 383, 6, RawBarHeight);

    private static int TrackWidth;

    private readonly Func<int>? GetMinValue;
    private readonly Func<int>? GetMaxValue;

    private int MinValue;
    private int MaxValue;

    public TrackBar(
        Func<int> loadValue,
        Action<int> saveValue,
        int? staticMinValue = null,
        int? staticMaxValue = null,
        Func<int>? getDynamicMinValue = null,
        Func<int>? getDynamicMaxValue = null,
        Action<int>? onValueChanged = null)
            : base(new Interaction.Draggable(), loadValue, saveValue, onValueChanged) {

      this.GetMinValue = getDynamicMinValue;
      this.GetMaxValue = getDynamicMaxValue;
      this.MinValue = staticMinValue ?? int.MinValue;
      this.MaxValue = staticMaxValue ?? int.MaxValue;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      TrackWidth = totalWidth / 3;
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
      position.Y += ScaledPadding;
      this.DrawFromCursors(sb, position, TrackSourceRect, height: ScaledBarHeight);

      float valuePercent = (this.Value - this.MinValue) / (float) (this.MaxValue - this.MinValue);
      position.X += valuePercent * (TrackWidth - ScaledBarWidth);
      this.DrawFromCursors(sb, position, BarSourceRect);
    }
  }

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
      string? tooltip = null) : base(name, tooltip) {

    TrackBar trackBar = new(
        loadValue, saveValue,
        staticMinValue, staticMaxValue,
        getDynamicMinValue, getDynamicMaxValue,
        onValueChanged);

    DynamicText label = DynamicText.CreateOptionLabel(
        (valueToString == null)
            ? () => trackBar.Value.ToString()
            : () => valueToString(trackBar.Value));

    this.AddSubWidget(trackBar,
        postDraw: (ref Vector2 position, int width, int _) => position.X += width + 24);
    this.AddSubWidget(label);
  }
}
