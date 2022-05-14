using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI;

internal interface IHoverable {
  bool IsHovering { get; set; }
}

internal interface IClickable {
  Action ClickAction { get; }
  string? ClickSoundName => null;
}

internal interface IDraggable {
  bool IsDragging { get; set; }
}

internal abstract partial class Widget {

  protected static int MousePositionX;
  protected static int MousePositionY;

  private record Interaction(IHoverable? Hoverable, IClickable? Clickable, IDraggable? Draggable) {

    private ButtonState MouseButtonState;

    internal void Update(Vector2 position, int width, int height) {
      MousePositionX = Game1.getOldMouseX();
      MousePositionY = Game1.getOldMouseY();

      bool isHovering = (position.X < MousePositionX) && (MousePositionX < (position.X + width))
          && (position.Y < MousePositionY) && (MousePositionY < (position.Y + height));

      if (this.Hoverable != null) {
        this.Hoverable.IsHovering = isHovering;
      }

      ButtonState oldMouseButtonState = this.MouseButtonState;
      this.MouseButtonState = Mouse.GetState().LeftButton;

      if (this.Clickable != null) {
        if (isHovering && (oldMouseButtonState == ButtonState.Pressed)
            && (this.MouseButtonState == ButtonState.Released)) {
          TryPlaySound(this.Clickable.ClickSoundName);
          this.Clickable.ClickAction();
        }
      }

      if (this.Draggable != null) {
        if (this.MouseButtonState == ButtonState.Released) {
          this.Draggable.IsDragging = false;
        } else if (isHovering && (oldMouseButtonState == ButtonState.Released)) {
          this.Draggable.IsDragging = true;
        }
      }
    }

    private static void TryPlaySound(string? soundName) {
      if (soundName != null) {
        Game1.playSound(soundName);
      }
    }
  }
}
