namespace Nuztalgia.StardewMods.Common.UI;

internal class Image : Widget {

  internal class WithCaption : Composite {

    private const int SpacingHeight = 8;

    internal WithCaption(
        string caption,
        Func<Texture2D[]> getSourceImages,
        Func<Rectangle[]>? getSourceRects = null,
        Func<Rectangle[]>? getDestRects = null,
        int? scale = null,
        int? fixedWidth = null,
        int? fixedHeight = null,
        Action? resetAction = null,
        Action? saveAction = null,
        Alignment? alignment = null)
            : base(alignment: alignment, linearMode: LinearMode.Vertical) {

      this.AddSubWidget(new Image(getSourceImages, getSourceRects, getDestRects, scale,
          fixedWidth, fixedHeight, resetAction, saveAction, alignment: Alignment.CenterX));
      this.AddSubWidget(Spacing.CreateVertical(SpacingHeight));
      this.AddSubWidget(StaticText.CreateImageCaption(caption));
    }
  }

  private readonly Func<Texture2D[]> GetSourceImages;
  private readonly Func<Rectangle[]?>? GetSourceRects;
  private readonly Func<Rectangle[]?>? GetDestRects;

  private readonly int Scale;
  private readonly int? FixedWidth;
  private readonly int? FixedHeight;

  private readonly Action? ResetAction;
  private readonly Action? SaveAction;

  internal Image(
      Func<Texture2D[]> getSourceImages,
      Func<Rectangle[]>? getSourceRects = null,
      Func<Rectangle[]>? getDestRects = null,
      int? scale = null,
      int? fixedWidth = null,
      int? fixedHeight = null,
      Action? resetAction = null,
      Action? saveAction = null,
      string? name = null,
      string? tooltip = null,
      Alignment? alignment = null)
          : base(name, tooltip, alignment) {

    this.GetSourceImages = getSourceImages;
    this.GetSourceRects = (getSourceRects == null) ? null : () => MatchImageRects(getSourceRects());
    this.GetDestRects = (getDestRects == null) ? null : () => MatchImageRects(getDestRects());
    this.Scale = scale ?? PixelZoom;
    this.FixedWidth = (fixedWidth == null) ? null : fixedWidth * this.Scale;
    this.FixedHeight = (fixedHeight == null) ? null : fixedHeight * this.Scale;
    this.ResetAction = resetAction;
    this.SaveAction = saveAction;

    Rectangle[]? MatchImageRects(Rectangle[] rects) {
      return ((rects.Length == getSourceImages().Length) || (rects.Length == 1)) ? rects : null;
    }
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    if ((this.FixedWidth != null) && (this.FixedHeight != null)) {
      return ((int) this.FixedWidth, (int) this.FixedHeight);
    } else {
      Rectangle[] rects = this.GetDestRects?.Invoke() ?? this.GetSourceRects?.Invoke()
          ?? this.GetSourceImages().Select(image => image.Bounds).ToArray();
      int width = this.FixedWidth ?? rects.Max(rect => rect.Right) * this.Scale;
      int height = this.FixedHeight ?? rects.Max(rect => rect.Bottom) * this.Scale;
      return (width, height);
    }
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    Texture2D[] images = this.GetSourceImages();
    Rectangle[]? sourceRects = this.GetSourceRects?.Invoke();
    Rectangle[]? destRects = this.GetDestRects?.Invoke();

    Action<Texture2D, Rectangle, int> draw = destRects?.Length switch {
      null or 0 => (image, sourceRect, _) => Draw(sb, position, image, sourceRect, this.Scale),
      1 => GetDrawActionForSingleDestRect(),
      _ => (image, sourceRect, i) => Draw(sb, image, sourceRect, AdjustDestRect(destRects[i]))
    };

    for (int i = 0; i < images.Length; ++i) {
      if ((sourceRects == null) || (sourceRects.Length == 0)) {
        draw(images[i], images[i].Bounds, i);
      } else if (sourceRects.Length == 1) {
        draw(images[i], sourceRects[0], i);
      } else {
        draw(images[i], sourceRects[i], i);
      }
    }

    Rectangle AdjustDestRect(Rectangle destRect) {
      destRect.X += (int) position.X;
      destRect.Y += (int) position.Y;
      destRect.Width *= this.Scale;
      destRect.Height *= this.Scale;
      return destRect;
    }

    Action<Texture2D, Rectangle, int> GetDrawActionForSingleDestRect() {
      Rectangle destRect = AdjustDestRect(destRects[0]);
      return (image, sourceRect, _) => Draw(sb, image, sourceRect, destRect);
    }
  }

  protected override void ResetState() {
    this.ResetAction?.Invoke();
  }

  protected override void SaveState() {
    this.SaveAction?.Invoke();
  }
}
