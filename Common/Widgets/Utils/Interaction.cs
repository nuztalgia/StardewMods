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

  private record Interaction(IHoverable? Hoverable, IClickable? Clickable, IDraggable? Draggable) {

    internal void Update(Vector2 position, int width, int height) {

      bool isHovering = (position.X < MousePositionX) && (MousePositionX < (position.X + width))
          && (position.Y < MousePositionY) && (MousePositionY < (position.Y + height));

      if (this.Hoverable != null) {
        this.Hoverable.IsHovering = isHovering;
      }

      if ((this.Clickable != null) && (!WasClickConsumed)
          && isHovering && WasMousePressed && !IsMousePressed) {
        TryPlaySound(this.Clickable.ClickSoundName);
        this.Clickable.ClickAction();
      }

      if (this.Draggable != null) {
        if (!IsMousePressed) {
          this.Draggable.IsDragging = false;
        } else if (isHovering && !WasMousePressed) {
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
