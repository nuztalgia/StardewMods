using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Image : Widget {

  internal class WithCaption : Composite {
    internal WithCaption(
        string caption,
        Texture2D sourceImage,
        Rectangle? sourceRect = null,
        int? scale = null,
        Alignment? alignment = null) : base(alignment, LinearMode.Vertical) {

      this.AddSubWidget(new Image(sourceImage, sourceRect, scale, Alignment.CenterX));
      this.AddSubWidget(Spacing.CreateVertical(height: 8));
      this.AddSubWidget(StaticText.CreateImageCaption(caption));
    }
  }

  private readonly Texture2D SourceImage;
  private readonly Rectangle SourceRect;
  private readonly int Scale;

  internal Image(
      Texture2D sourceImage,
      Rectangle? sourceRect = null,
      int? scale = null,
      Alignment? alignment = null) : base(alignment) {

    this.SourceImage = sourceImage;
    this.SourceRect = sourceRect ?? sourceImage.Bounds;
    this.Scale = scale ?? PixelZoom;
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return (this.SourceRect.Width * this.Scale, this.SourceRect.Height * this.Scale);
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    Draw(sb, position, this.SourceImage, this.SourceRect, this.Scale);
  }
}
