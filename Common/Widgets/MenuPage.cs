using Microsoft.Xna.Framework.Input;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal sealed class MenuPage : Composite {

    private const int VerticalSpacing = 16;

    internal MenuPage(
        IEnumerable<Widget> orderedWidgets,
        IDictionary<Widget, Func<bool>>? hideableWidgets)
            : base(hideableWidgets) {

      orderedWidgets.ForEach((Widget widget) => this.AddSubWidget(widget,
          postDraw: (ref Vector2 position, int _, int _) => position.Y += VerticalSpacing));
    }

    internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
      gmcmApi.AddComplexOption(
          mod: modManifest,
          name: () => string.Empty,
          draw: this.OnMenuDraw,
          beforeMenuOpened: this.OnMenuOpening,
          beforeMenuClosed: this.RefreshStateAndSize,
          beforeReset: this.RefreshStateAndSize,
          beforeSave: this.SaveState,
          height: () => this.Height);
    }

    private void OnMenuDraw(SpriteBatch sb, Vector2 position) {
      MousePositionX = Game1.getOldMouseX();
      MousePositionY = Game1.getOldMouseY();

      WasMousePressed = IsMousePressed;
      IsMousePressed = Mouse.GetState().LeftButton == ButtonState.Pressed;

      // We register a click when the mouse is released - not when it's first pressed.
      WasClickConsumed = WasMousePressed && !IsMousePressed
          && (ActiveOverlay?.TryConsumeClick() == true);

      ActiveOverlayDrawPosition = default;

      this.Draw(sb, position, null, null);

      if (ActiveOverlayDrawPosition != default) {
        (ActiveOverlay as Widget)?.Draw(sb, ActiveOverlayDrawPosition);
      }

      (ActiveTooltip as Widget)?.Draw(sb, default); // Tooltips don't need a draw position.
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
  }
}
