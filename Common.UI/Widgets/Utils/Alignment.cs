using Microsoft.Xna.Framework;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Alignment {

  private enum AlignmentX {
    None, Left, Right, Center
  }

  private enum AlignmentY {
    None, Center
  }

  internal static readonly Alignment Left = new(AlignmentX.Left, AlignmentY.None);
  internal static readonly Alignment Right = new(AlignmentX.Right, AlignmentY.None);
  internal static readonly Alignment Center = new(AlignmentX.Center, AlignmentY.Center);

  private readonly AlignmentX AlignX;
  private readonly AlignmentY AlignY;

  private Alignment(AlignmentX alignX, AlignmentY alignY) {
    this.AlignX = alignX;
    this.AlignY = alignY;
  }

  internal void Align(
      ref Vector2 position,
      int width,
      int height,
      int containerWidth,
      int containerHeight,
      int? leftAdjustment = null,
      int? rightAdjustment = null) {

    switch (this.AlignX) {
      case AlignmentX.Left:
        position.X -= leftAdjustment ?? (containerWidth / 2);
        break;
      case AlignmentX.Right:
        position.X += (rightAdjustment ?? (containerWidth / 2)) - width;
        break;
      case AlignmentX.Center:
        position.X += (containerWidth - width) / 2;
        break;
    }

    if (this.AlignY == AlignmentY.Center) {
      position.Y += (containerHeight - height) / 2;
    }
  }
}
