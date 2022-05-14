using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal abstract class Option<TValue> : Widget where TValue : IEquatable<TValue> {

    internal TValue Value { get; private protected set; }

    private readonly Func<TValue> LoadValue;
    private readonly Action<TValue> SaveValue;
    private readonly Action<TValue>? OnValueChanged;

    protected Option(
        Func<TValue> loadValue,
        Action<TValue> saveValue,
        Action<TValue>? onValueChanged = null,
        string? name = null,
        string? tooltip = null) : base(name, tooltip) {

      this.Value = loadValue();

      if (this.Value is not (bool or float or int or string)) {
        throw new InvalidOperationException($"'{typeof(TValue)}' is not a supported option type.");
      }

      this.LoadValue = loadValue;
      this.SaveValue = saveValue;
      this.OnValueChanged = onValueChanged;
    }

    protected abstract void DrawOption(SpriteBatch sb, Vector2 position);

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      TValue previousValue = this.Value;

      this.DrawOption(sb, position);

      if (!previousValue.Equals(this.Value)) {
        this.OnValueChanged?.Invoke(this.Value);
      }
    }

    protected override sealed void ResetState() {
      this.Value = this.LoadValue();
    }

    protected override sealed void SaveState() {
      this.SaveValue(this.Value);
    }
  }
}
