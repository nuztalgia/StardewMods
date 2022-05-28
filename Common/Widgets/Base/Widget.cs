namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  protected const int DefaultHeight = 48;

  private const int MinContainerWidth = 1200;
  private const int ViewportPaddingX = 200;
  private const int ViewportPaddingY = 244;

  private static int ViewportWidth;
  private static int ViewportHeight;
  private static int ContainerWidth;
  private static int ContainerHeight;

  private static bool WasClickConsumed;
  private static bool WasMousePressed;
  private static bool IsMousePressed;

  protected static float MousePositionX { get; private set; }
  protected static float MousePositionY { get; private set; }

  private readonly Widget? NameLabel;
  private readonly Alignment? Alignable;
  private readonly Interaction? Interactable;
  private readonly Func<bool> IsDrawable;

  private int Width;
  private int Height;

  protected Widget(
      string? name = null,
      string? tooltip = null,
      Alignment? alignment = null) {

    this.NameLabel = Label.Create(labelText: name, tooltipText: tooltip);
    this.Alignable = alignment;

    if (this is IHoverable or IClickable or IDraggable) {
      this.Interactable = new(this as IHoverable, this as IClickable, this as IDraggable);
    }

    this.IsDrawable = (this is IOverlayable)
        ? () => this == ActiveOverlay
        : () => true;
  }

  protected virtual void ResetState() { }

  protected virtual void SaveState() { }

  protected abstract (int width, int height) UpdateDimensions(int totalWidth);

  protected abstract void Draw(SpriteBatch sb, Vector2 position);

  private void Draw(
      SpriteBatch sb, Vector2 position, int? containerWidth = null, int? containerHeight = null) {
    if (this.IsDrawable()) {
      this.Alignable?.Align(
          ref position, this.Width, this.Height,
          containerWidth ?? ContainerWidth, containerHeight ?? ContainerHeight,
          (containerWidth == null) || (containerHeight == null));
      this.Interactable?.Update(position, this.Width, this.Height);
      this.Draw(sb, position);
    }
  }

  private void RefreshStateAndSize() {
    ClearActiveOverlay();
    this.ResetState();
    (this.Width, this.Height) = this.UpdateDimensions(ContainerWidth);
  }

  protected static void TryPlaySound(string? soundName) {
    if (soundName != null) {
      Game1.playSound(soundName);
    }
  }

  private static void UpdateStaticDimensions() {
    if ((ViewportWidth != Game1.uiViewport.Width) || (ViewportHeight != Game1.uiViewport.Height)) {
      ViewportWidth = Game1.uiViewport.Width;
      ViewportHeight = Game1.uiViewport.Height;
      ContainerWidth = Math.Min(ViewportWidth - ViewportPaddingX, MinContainerWidth);
      ContainerHeight = ViewportHeight - ViewportPaddingY;
    }
  }
}
