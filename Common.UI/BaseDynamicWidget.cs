using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class BaseWidgetWithValue<TValue> : BaseWidget
    where TValue : notnull, IEquatable<TValue> {

  protected TValue Value { get; set; } = default!;
  protected string ValueText => this.ValueToString.Invoke(this.Value);

  private readonly Func<TValue> GetValue;
  private readonly Action<TValue> SaveValue;

  private readonly Func<TValue, string> ValueToString;
  private readonly Action<TValue>? OnValueChange;

  protected BaseWidgetWithValue(
      string name,
      Func<TValue> getValue,
      Action<TValue> saveValue,
      Func<TValue, string>? valueToString = null,
      Action<TValue>? onValueChange = null,
      string? tooltip = null)
          : base(name, tooltip) {
    this.GetValue = getValue;
    this.SaveValue = saveValue;
    this.ValueToString = valueToString ?? (value => value.ToString() ?? string.Empty);
    this.OnValueChange = onValueChange;
  }

  protected abstract void UpdateState(Vector2 position);

  protected abstract void DrawState(SpriteBatch sb, Vector2 position);

  protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
    TValue previousValue = this.Value;

    this.UpdateState(position);
    this.DrawState(sb, position);

    if (!previousValue.Equals(this.Value)) {
      this.OnValueChange?.Invoke(this.Value);
    }
  }

  protected override void ResetState() {
    this.Value = this.GetValue();
  }

  protected override void SaveState() {
    this.SaveValue(this.Value);
  }
}
