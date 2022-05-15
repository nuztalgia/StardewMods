using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Spacing : Widget {

  internal static readonly Spacing DefaultHorizontal = new(width: DefaultHeight / 2, height: 1);
  internal static readonly Spacing DefaultVertical = new(width: 1, height: DefaultHeight);

  private readonly int Width;
  private readonly int Height;

  private Spacing(int width, int height) {
    this.Width = width;
    this.Height = height;
  }

  internal static Spacing CreateHorizontal(int width) {
    return (width == DefaultHorizontal.Width) ? DefaultHorizontal : new Spacing(width, height: 1);
  }

  internal static Spacing CreateVertical(int height) {
    return (height == DefaultVertical.Height) ? DefaultVertical : new Spacing(width: 1, height);
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return (this.Width, this.Height);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    // No-op. This widget is just empty space.
  }
}
