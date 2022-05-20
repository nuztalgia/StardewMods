namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  protected const int DefaultHeight = 48;

  private const int MinTotalWidth = 1200;
  private const int ViewportPadding = 200;

  private static int ViewportWidth;
  private static int TotalWidth;

  private static int LeftAdjustment;
  private static int RightAdjustment;

  private readonly string Name;
  private readonly string? Tooltip;
  private readonly Alignment? Alignable;
  private readonly Interaction? Interactable;

  private int Width;
  private int Height;

  protected Widget(string? name = null, string? tooltip = null, Alignment? alignment = null) {
    this.Name = name ?? string.Empty;
    this.Tooltip = tooltip;
    this.Alignable = alignment;

    if (this is IHoverable or IClickable or IDraggable) {
      this.Interactable = new(this as IHoverable, this as IClickable, this as IDraggable);
    }
  }

  protected Widget(Alignment? alignment) : this(name: null, tooltip: null, alignment) { }

  internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
    gmcmApi.AddComplexOption(
        mod: modManifest,
        name: () => this.Name,
        draw: (sb, position) => this.Draw(sb, position, null, null),
        tooltip: (this.Tooltip is null) ? null : () => this.Tooltip,
        beforeMenuOpened: this.OnMenuOpening,
        beforeMenuClosed: this.RefreshStateAndSize,
        beforeReset: this.RefreshStateAndSize,
        beforeSave: this.SaveState,
        height: () => this.Height
    );
  }

  protected virtual void ResetState() { }

  protected virtual void SaveState() { }

  protected abstract (int width, int height) UpdateDimensions(int totalWidth);

  protected abstract void Draw(SpriteBatch sb, Vector2 position);

  private void Draw(
      SpriteBatch sb, Vector2 position, int? containerWidth = null, int? containerHeight = null) {
    this.Alignable?.Align(
        ref position, this.Width, this.Height,
        containerWidth ?? TotalWidth, containerHeight ?? DefaultHeight,
        LeftAdjustment, RightAdjustment, (containerWidth == null) || (containerHeight == null));
    this.Interactable?.Update(position, this.Width, this.Height);
    this.Draw(sb, position);
  }

  private void OnMenuOpening() {
    if (ViewportWidth != Game1.uiViewport.Width) {
      ViewportWidth = Game1.uiViewport.Width;
      TotalWidth = Math.Min(ViewportWidth - ViewportPadding, MinTotalWidth);
      LeftAdjustment = (TotalWidth / 2) + 8;
      RightAdjustment = (TotalWidth / 2) - 8;
    }
    this.RefreshStateAndSize();
  }

  private void RefreshStateAndSize() {
    this.ResetState();
    (this.Width, this.Height) = this.UpdateDimensions(TotalWidth);
  }
}
