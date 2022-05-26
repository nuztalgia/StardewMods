using StardewValley.Menus;

namespace Nuztalgia.StardewMods.Common.UI;

internal class Tooltip : Widget, IOverlayable {

  private readonly string Text;
  private readonly string Title;

  internal Tooltip(string text, string title) {
    this.Text = text;
    this.Title = title;
  }

  public bool TryConsumeClick() {
    return false; // Tooltips are never clickable.
  }

  internal void SetHoverState(bool isHovering) {
    this.SetOverlayStatus(isActive: isHovering);
  }

  protected override (int width, int height) UpdateDimensions(int totalWidth) {
    return IOverlayable.Dimensions;
  }

  protected override void Draw(SpriteBatch sb, Vector2 position) {
    IClickableMenu.drawToolTip(sb, hoverText: this.Text, hoverTitle: this.Title, hoveredItem: null);
  }
}
