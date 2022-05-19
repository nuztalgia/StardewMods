using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Spacing : Widget {

  private const int DefaultWidth = DefaultHeight / 2;

  internal static readonly Spacing DefaultHorizontal = new(width: DefaultWidth, height: 1);
  internal static readonly Spacing DefaultVertical = new(width: 1, height: DefaultHeight);

  private readonly int Width;
  private readonly int Height;

  private Spacing(int width, int height) {
    this.Width = width;
    this.Height = height;
  }

  internal static Spacing CreateHorizontal(int? width = null) {
    return (width is null or DefaultWidth)
        ? DefaultHorizontal
        : new Spacing((int) width, height: 1);
  }

  internal static Spacing CreateVertical(int? height = null) {
    return (height is null or DefaultHeight)
        ? DefaultVertical
        : new Spacing(width: 1, (int) height);
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return (this.Width, this.Height);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    // No-op. This widget is just empty space.
  }
}
