namespace Nuztalgia.StardewMods.Common.UI;

internal interface IOverlayable {

  // Overlays do not occupy any actual "real estate" on the page.
  static readonly (int width, int height) Dimensions = (0, 0);

  bool HasScrollFocus => false;

  bool TryConsumeClick();

  void OnScrolled(int scrollDelta) { }

  void OnDismissed() { }
}

internal static class IOverlayableExtensions {

  internal static IOverlayable? ActiveOverlay { get; private set; }
  internal static IOverlayable? ActiveTooltip { get; private set; }

  internal static void SetOverlayStatus<TOverlay>(this TOverlay widget, bool isActive)
      where TOverlay : Widget, IOverlayable {

    if (widget is Tooltip) {
      ActiveTooltip = isActive ? widget : null;
      return;
    }

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

  internal static void ClearActiveOverlay() {
    ActiveOverlay?.OnDismissed();
    ActiveOverlay = null;
  }
}

internal abstract partial class Widget {

  private static Vector2 ActiveOverlayDrawPosition;

  private static IOverlayable? ActiveOverlay => IOverlayableExtensions.ActiveOverlay;
  private static IOverlayable? ActiveTooltip => IOverlayableExtensions.ActiveTooltip;

  private static void ClearActiveOverlay() {
    IOverlayableExtensions.ClearActiveOverlay();
  }
}
