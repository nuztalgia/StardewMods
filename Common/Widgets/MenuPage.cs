using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal sealed class MenuPage : Composite {

#pragma warning disable CS8618 // Non-nullable field must contain non-null value. Set in Initialize.
    internal static IInputEvents InputEvents { get; private set; }
#pragma warning restore CS8618

    private const int VerticalSpacing = 16;

    internal MenuPage(
        IEnumerable<Widget> orderedWidgets,
        IDictionary<Widget, Func<bool>>? hideableWidgets) : base(
            hideableWidgets: hideableWidgets, linearMode: LinearMode.Vertical, isFullWidth: true) {

      orderedWidgets.ForEach((Widget widget) => this.AddSubWidget(widget,
          postDraw: (ref Vector2 position, int _, int _) => position.Y += VerticalSpacing));
    }

    internal static void Initialize(IInputEvents inputEvents) {
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
        (ActiveOverlay as Widget)?.Draw(sb, ActiveOverlayDrawPosition);
      }

      (ActiveTooltip as Widget)?.Draw(sb, default); // Tooltips don't need a draw position.
    }

    private void OnMenuOpening() {
      UpdateStaticWidths();
      this.RefreshStateAndSize();
      InputEvents.MouseWheelScrolled += OnMouseWheelScrolled;
    }

    private void OnMenuClosing() {
      InputEvents.MouseWheelScrolled -= OnMouseWheelScrolled;
      this.RefreshStateAndSize();
    }

    private static void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs args) {
      ActiveOverlay?.OnScrolled(scrollDelta: args.Delta);
    }
  }
}
