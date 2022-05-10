using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI;

internal static class Extensions {

  internal interface IDraggable {
    ButtonState MouseButtonState { get; set; }
    (int X, int Y) MousePosition { get; set; }
    bool IsDragging { get; set; }
  }

  internal static void UpdateMouseState<TWidget>(
      this TWidget widget,
      Vector2 widgetDrawPosition,
      int? customDraggableWidth = null,
      int? customDraggableHeight = null)
          where TWidget : BaseWidget, IDraggable {

    ButtonState newButtonState = Mouse.GetState().LeftButton;
    widget.MousePosition = (Game1.getOldMouseX(), Game1.getOldMouseY());

    if (newButtonState == ButtonState.Released) {
      widget.IsDragging = false;
    } else if (widget.MouseButtonState == ButtonState.Released) {
      Rectangle draggableRect = new(
          (int) widgetDrawPosition.X, (int) widgetDrawPosition.Y,
          customDraggableWidth ?? widget.Width, customDraggableHeight ?? widget.Height);
      if (draggableRect.Contains(widget.MousePosition.X, widget.MousePosition.Y)) {
        widget.IsDragging = true;
      }
    }

    widget.MouseButtonState = newButtonState;
  }
}
