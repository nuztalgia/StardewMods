namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  protected interface IOverlayable {
    IClickable? ClickableTrigger => null;

    void SetOverlayStatus(bool isActive) {
      Widget.SetOverlayStatus(this, isActive);
    }

    void OnDismissed() { }
  }

  protected const int DefaultHeight = 48;

  private const int MinTotalWidth = 1200;
  private const int ViewportPadding = 200;

  private static int ViewportWidth;
  private static int TotalWidth;

  private static IOverlayable? ActiveOverlay;
  private static Vector2 ActiveOverlayPosition;

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

  // TODO: Move this to MenuPage after existing menus are migrated to the new framework.
  internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
    gmcmApi.AddComplexOption(
        mod: modManifest,
        name: () => string.Empty,
        draw: (sb, position) => this.Draw(sb, position, null, null),
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
    if (this.IsDrawable()) {
      this.Alignable?.Align(
          ref position, this.Width, this.Height,
          containerWidth ?? TotalWidth, containerHeight ?? DefaultHeight,
          (containerWidth == null) || (containerHeight == null));
      this.Interactable?.Update(position, this.Width, this.Height);
      this.Draw(sb, position);
    }
  }

  private void OnMenuOpening() {
    if (ViewportWidth != Game1.uiViewport.Width) {
      ViewportWidth = Game1.uiViewport.Width;
      TotalWidth = Math.Min(ViewportWidth - ViewportPadding, MinTotalWidth);
    }
    this.RefreshStateAndSize();
  }

  private void RefreshStateAndSize() {
    ClearActiveOverlay();
    this.ResetState();
    (this.Width, this.Height) = this.UpdateDimensions(TotalWidth);
  }

  private static void SetOverlayStatus(IOverlayable widget, bool isActive) {
    if (isActive) {
      if (ActiveOverlay == widget) {
        return;
      } else if (ActiveOverlay != null) {
        ActiveOverlay?.OnDismissed();
      }
      ActiveOverlay = widget;
    } else if (ActiveOverlay == widget) {
      ClearActiveOverlay();
    }
  }

  private static void ClearActiveOverlay() {
    ActiveOverlay?.OnDismissed();
    ActiveOverlay = null;
    ActiveOverlayPosition = default;
  }
}
