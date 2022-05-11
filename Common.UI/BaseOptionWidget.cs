using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class BaseWidget {

  internal abstract class Option<TValue> : BaseWidget where TValue : IEquatable<TValue> {

    private readonly Func<TValue> LoadValue;
    private readonly Action<TValue> SaveValue;
    private readonly Action<TValue>? OnValueChanged;

    private TValue Value;

    protected Option(
        string name,
        Func<TValue> loadValue,
        Action<TValue> saveValue,
        Action<TValue>? onValueChanged = null,
        string? tooltip = null,
        Interaction? interaction = null)
            : base(name, tooltip, interaction) {

      this.Value = loadValue();

      if (this.Value is not (bool or float or int or string)) {
        throw new InvalidOperationException($"'{typeof(TValue)}' is not a supported option type.");
      }

      this.LoadValue = loadValue;
      this.SaveValue = saveValue;
      this.OnValueChanged = onValueChanged;
    }

    protected abstract TValue UpdateValue(Vector2 position, TValue previousValue);

    protected abstract void Draw(SpriteBatch sb, Vector2 position, TValue currentValue);

    protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
      TValue previousValue = this.Value;

      this.Value = this.UpdateValue(position, previousValue);
      this.Draw(sb, position, this.Value);

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
