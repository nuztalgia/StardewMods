using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Checkbox : OptionWidget<bool>, IClickable {

  private const int RawBoxSize = 9;
  private const int ScaledBoxSize = RawBoxSize * PixelZoom;

  private static readonly Rectangle CheckedSourceRect = new(236, 425, RawBoxSize, RawBoxSize);
  private static readonly Rectangle UncheckedSourceRect = new(227, 425, RawBoxSize, RawBoxSize);

  public Action ClickAction { get; }
  public string ClickSoundName => "drumkit6";

  internal Checkbox(
      string name,
      Func<bool> loadValue,
      Action<bool> saveValue,
      Action<bool>? onValueChanged = null,
      string? tooltip = null)
          : base(loadValue, saveValue, onValueChanged, name, tooltip) {
    this.ClickAction = () => this.Value = !this.Value;
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return (ScaledBoxSize, ScaledBoxSize);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    DrawFromCursors(sb, position, this.Value ? CheckedSourceRect : UncheckedSourceRect);
  }
}
