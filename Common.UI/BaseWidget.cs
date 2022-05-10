using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI; 

internal abstract class BaseWidget {

  protected const int DefaultHeight = 48;
  protected const int PixelZoom = Game1.pixelZoom;

  private const int MinTotalWidth = 1200;
  private const int ViewportPadding = 200;

  protected static readonly SpriteFont MainFont = Game1.dialogueFont;
  protected static readonly SpriteFont SmallFont = Game1.smallFont;
  protected static readonly Texture2D CursorsImage = Game1.mouseCursors;

  protected virtual string Name { get; private init; }
  protected virtual string? Tooltip { get; private init; }

  internal virtual int Width { get; private set; }
  internal virtual int Height { get; private set; }

  private static int ViewportWidth;
  private static int TotalWidth;

  protected BaseWidget(string? name = null, string? tooltip = null) {
    this.Name = name ?? string.Empty;
    this.Tooltip = tooltip;
  }

  internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
    gmcmApi.AddComplexOption(
        mod: modManifest,
        name: () => this.Name,
        draw: this.Draw,
        tooltip: (this.Tooltip is null) ? null : () => this.Tooltip,
        beforeMenuOpened: this.OnMenuOpening,
        beforeMenuClosed: this.RefreshStateAndSize,
        beforeReset: this.RefreshStateAndSize,
        beforeSave: this.SaveState,
        height: () => this.Height
    );
  }

  protected abstract void Draw(SpriteBatch sb, Vector2 position);

  protected virtual void ResetState() { }

  protected virtual void SaveState() { }

  protected virtual (int width, int height) CalculateDimensions(int totalWidth) {
    return (totalWidth, DefaultHeight);
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
    (this.Width, this.Height) = this.CalculateDimensions(TotalWidth);
  }
}
