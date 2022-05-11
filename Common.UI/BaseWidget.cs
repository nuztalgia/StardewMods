using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI; 

internal abstract partial class BaseWidget {

  protected const int DefaultHeight = 48;
  protected const int PixelZoom = Game1.pixelZoom;

  private const int MinTotalWidth = 1200;
  private const int ViewportPadding = 200;

  protected static readonly SpriteFont MainFont = Game1.dialogueFont;
  protected static readonly SpriteFont SmallFont = Game1.smallFont;
  protected static readonly Texture2D Cursors = Game1.mouseCursors;

  private static int ViewportWidth;
  private static int TotalWidth;

  // The widget's Interaction must have been set appropriately in order to use these without error.
  protected bool IsHovering => (this.Interaction as Interaction.Clickable)!.IsHovering;
  protected bool IsDragging => (this.Interaction as Interaction.Draggable)!.IsDragging;
  protected Point MousePosition => (this.Interaction as Interaction.Draggable)!.MousePosition;

  private readonly string Name;
  private readonly string? Tooltip;
  private readonly Interaction? Interaction;

  protected int Width { get; private set; }
  protected int Height { get; private set; }

  protected BaseWidget(
      string? name = null, string? tooltip = null, Interaction? interaction = null) {
    this.Name = name ?? string.Empty;
    this.Tooltip = tooltip;
    this.Interaction = interaction;
  }

  internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
    gmcmApi.AddComplexOption(
        mod: modManifest,
        name: () => this.Name,
        draw: this.InteractiveDraw,
        tooltip: (this.Tooltip is null) ? null : () => this.Tooltip,
        beforeMenuOpened: this.OnMenuOpening,
        beforeMenuClosed: this.RefreshStateAndSize,
        beforeReset: this.RefreshStateAndSize,
        beforeSave: this.SaveState,
        height: () => this.Height
    );
  }

  protected virtual void ResetState() { }

  protected virtual void SaveState() { }

  protected virtual (int width, int height) UpdateDimensions(int totalWidth) {
    return (totalWidth, DefaultHeight);
  }

  protected abstract void Draw(SpriteBatch sb, Vector2 position);

  private void InteractiveDraw(SpriteBatch sb, Vector2 position) {
    this.Interaction?.Update(new((int) position.X, (int) position.Y, this.Width, this.Height));
    this.Draw(sb, position);
  }

  private void OnMenuOpening() {
    if (ViewportWidth != Game1.uiViewport.Width) {
      ViewportWidth = Game1.uiViewport.Width;
      TotalWidth = Math.Min(ViewportWidth - ViewportPadding, MinTotalWidth);
    }
    this.RefreshStateAndSize();
  }

  private void RefreshStateAndSize() {
    this.ResetState();
    (this.Width, this.Height) = this.UpdateDimensions(TotalWidth);
  }
}
