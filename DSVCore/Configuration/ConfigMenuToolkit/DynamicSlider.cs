using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuztalgia.StardewMods.Common;
using StardewValley;

namespace Nuztalgia.StardewMods.DSVCore;

internal class DynamicSlider {

  internal interface IDataSource {
    abstract Func<int> GetMinValue { get; init; }
    abstract Func<int> GetMaxValue { get; init; }
    abstract int Value { get; set; }

    void OnValueChanged(int newValue) { }
  }

  internal const int TotalHeight = ScaledHeight + (ScaledPadding * 2);

  private const int ScaledBarWidth = RawBarWidth * Game1.pixelZoom;
  private const int ScaledHeight = RawHeight * Game1.pixelZoom;
  private const int ScaledPadding = ScaledHeight / 2;

  private const int RawBarWidth = 10;
  private const int RawHeight = 6;

  private static readonly Rectangle BarSourceRect = new(420, 441, RawBarWidth, RawHeight);
  private static readonly Rectangle TrackSourceRect = new(403, 383, RawHeight, RawHeight);

  private static int TrackWidth => Math.Min(1200, Game1.uiViewport.Width - 200) / 3;

  private readonly IDataSource DataSource;

  private bool IsDragging = false;
  private ButtonState ButtonState = ButtonState.Released;

  internal DynamicSlider(IDataSource dataSource) {
    this.DataSource = dataSource;
  }

  internal void Draw(SpriteBatch sb, Vector2 position) {
    this.UpdateMouseState(position, out float mousePositionPercent);
    this.UpdateCurrentValue(mousePositionPercent, out float currentValuePercent);

    sb.DrawString(
        this.DataSource.Value.ToString(),
        new Vector2(position.X + TrackWidth + (ScaledPadding * 2), position.Y));

    position.Y += ScaledPadding;

    Vector2 barPosition =
        new(position.X + (currentValuePercent * (TrackWidth - ScaledBarWidth)), position.Y);

    sb.DrawTextureBox(Game1.mouseCursors, position, TrackSourceRect, TrackWidth, ScaledHeight);
    sb.Draw(Game1.mouseCursors, barPosition, BarSourceRect);
  }

  private void UpdateMouseState(Vector2 position, out float mousePositionPercent) {
    Rectangle trackBounds = new((int) position.X, (int) position.Y, TrackWidth, TotalHeight);
    ButtonState currentButtonState = Mouse.GetState().LeftButton;
    (int mouseX, int mouseY) = (Game1.getOldMouseX(), Game1.getOldMouseY());

    if (currentButtonState == ButtonState.Released) {
      this.IsDragging = false;
    } else if (trackBounds.Contains(mouseX, mouseY)
        && (this.ButtonState == ButtonState.Released)) {
      this.IsDragging = true;
    }

    this.ButtonState = currentButtonState;
    mousePositionPercent = (mouseX - position.X) / TrackWidth;
  }

  private void UpdateCurrentValue(float mousePositionPercent, out float currentValuePercent) {
    int minValue = Math.Min(this.DataSource.GetMinValue(), this.DataSource.GetMaxValue());
    int maxValue = Math.Max(this.DataSource.GetMinValue(), this.DataSource.GetMaxValue());
    int currentValue = this.DataSource.Value;

    if (this.IsDragging) {
      currentValue = Math.Clamp(
          (int) (mousePositionPercent * (maxValue - minValue)) + minValue, minValue, maxValue);
    }

    currentValuePercent = (currentValue - minValue) / (float) (maxValue - minValue);

    if (currentValue != this.DataSource.Value) {
      this.DataSource.OnValueChanged(currentValue);
      this.DataSource.Value = currentValue;
    }
  }
}
