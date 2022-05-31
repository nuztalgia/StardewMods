using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal sealed class MenuPage : Composite {

#pragma warning disable CS8618 // Non-nullable field must contain non-null value. Set in Initialize.
    internal static IInputEvents InputEvents { get; private set; }
#pragma warning restore CS8618

    private const int VerticalSpacing = 16;

    private static MenuPage? ActivePage = null;
    private static bool ActiveOverlayHasScrollFocus =>
        (ActivePage != null) && (ActiveOverlay != null) && ActiveOverlay.HasScrollFocus;

    internal MenuPage(
        IEnumerable<Widget> orderedWidgets,
        IDictionary<Widget, Func<bool>>? hideableWidgets) : base(
            hideableWidgets: hideableWidgets,
            linearMode: LinearMode.Vertical,
            isFullWidth: true,
            isHeightManager: true) {

      orderedWidgets.ForEach((Widget widget) => this.AddSubWidget(widget,
          postDraw: (ref Vector2 position, int _, int _) => position.Y += VerticalSpacing));
    }

    internal static void Initialize(string modId, IInputEvents inputEvents) {
#if HARMONY
      // This patch will only affect GMCM pages that were built using this class (MenuPage).
      new HarmonyPatcher(modId, typeof(MenuPage))
          .ForMethod("SpaceShared.UI.Scrollbar", "ScrollBy")?
          .ApplyPrefixPatch(nameof(Scrollbar_ScrollBy_Prefix));
#endif
      InputEvents = inputEvents;
    }

    internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
      gmcmApi.AddComplexOption(
          mod: modManifest,
          name: () => string.Empty,
          draw: this.OnMenuDraw,
          beforeMenuOpened: this.OnMenuOpening,
          beforeMenuClosed: this.OnMenuClosing,
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
        Widget widget = (ActiveOverlay as Widget)!;
        (widget.Width, widget.Height) = widget.UpdateDimensions(ContainerWidth);
        widget.Draw(sb, ActiveOverlayDrawPosition);
      }

      (ActiveTooltip as Widget)?.Draw(sb, default); // Tooltips don't need a draw position.
    }

    private void OnMenuOpening() {
      UpdateStaticDimensions();
      this.RefreshStateAndSize();
      InputEvents.MouseWheelScrolled += OnMouseWheelScrolled;
      ActivePage = this;
    }

    private void OnMenuClosing() {
      ActivePage = null;
      InputEvents.MouseWheelScrolled -= OnMouseWheelScrolled;
      this.RefreshStateAndSize();
    }

    private static void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs args) {
      if (ActiveOverlayHasScrollFocus) {
        ActiveOverlay?.OnScrolled(scrollDelta: args.Delta);
      }
    }

#if HARMONY
    private static bool Scrollbar_ScrollBy_Prefix(ref int amount) {
      if (ActiveOverlayHasScrollFocus) {
        // When there's an active overlay with scroll focus, disregard external page scrolls.
        amount = 0; 
      } else if ((ActivePage != null) && (ActivePage.Height < ContainerHeight)) {
        // On non-scrollable pages, force the scroll position to the top (and keep it there).
        amount = int.MinValue;
      }
      return true;
    }
#endif
  }
}
