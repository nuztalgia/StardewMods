using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Spacing : Widget {

  private readonly int Height;

  internal Spacing(int height = DefaultHeight) {
    this.Height = height;
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return (totalWidth, this.Height);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) { }
}
