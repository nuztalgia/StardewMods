using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class Interaction {

  internal sealed class Clickable : Interaction {
    internal bool IsHovering { get; private set; }

    private readonly Action ClickAction;
    private readonly string? ClickSoundName;

    public Clickable(Action clickAction, string? clickSoundName = null) {
      this.ClickAction = clickAction;
      this.ClickSoundName = clickSoundName;
    }

    protected override void Update(ButtonState oldButtonState, bool isHovering, Point _) {
      if (isHovering && (oldButtonState == ButtonState.Pressed)
          && (this.MouseButtonState == ButtonState.Released)) {
        TryPlaySound(this.ClickSoundName);
        this.ClickAction();
      }
      this.IsHovering = isHovering;
    }
  }

  internal sealed class Draggable : Interaction {
    internal Point MousePosition { get; private set; }
    internal bool IsDragging { get; private set; }

    protected override void Update(
        ButtonState oldButtonState, bool isHovering, Point mousePosition) {
      if (this.MouseButtonState == ButtonState.Released) {
        this.IsDragging = false;
      } else if (isHovering && (oldButtonState == ButtonState.Released)) {
        this.IsDragging = true;
      }
      this.MousePosition = mousePosition;
    }
  }

  private ButtonState MouseButtonState;

  internal void Update(Rectangle interactionArea) {
    ButtonState oldButtonState = this.MouseButtonState;
    this.MouseButtonState = Mouse.GetState().LeftButton;

    Point mousePosition = new(Game1.getOldMouseX(), Game1.getOldMouseY());
    this.Update(oldButtonState, interactionArea.Contains(mousePosition), mousePosition);
  }

  protected abstract void Update(ButtonState oldButtonState, bool isHovering, Point mousePosition);

  protected static void TryPlaySound(string? soundName) {
    if (soundName != null) {
      Game1.playSound(soundName);
    }
  }
}
