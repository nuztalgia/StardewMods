using System;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class OptionWidget<TValue> : Widget where TValue : IEquatable<TValue> {

  protected TValue Value {
    get => this.ValueField;
    set {
      if (!value.Equals(this.Value)) {
        this.OnValueChanged?.Invoke(value);
        this.ValueField = value;
      }
    }
  }

  private readonly Func<TValue> LoadValue;
  private readonly Action<TValue> SaveValue;
  private readonly Action<TValue>? OnValueChanged;

  private TValue ValueField;

  protected OptionWidget(
      Func<TValue> loadValue,
      Action<TValue> saveValue,
      Action<TValue>? onValueChanged = null,
      string? name = null,
      string? tooltip = null) : base(name, tooltip) {

    this.ValueField = loadValue();

    if (this.ValueField is not (bool or float or int or string)) {
      throw new InvalidOperationException($"'{typeof(TValue)}' is not a supported option type.");
    }

    this.LoadValue = loadValue;
    this.SaveValue = saveValue;
    this.OnValueChanged = onValueChanged;
  }

  protected override sealed void ResetState() {
    this.Value = this.LoadValue();
  }

  protected override sealed void SaveState() {
    this.SaveValue(this.Value);
  }
}
