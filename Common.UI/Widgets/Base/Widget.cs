using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI; 

internal abstract partial class Widget {

  protected const int DefaultHeight = 48;

  private const int MinTotalWidth = 1200;
  private const int ViewportPadding = 200;

  private static int ViewportWidth;
  private static int TotalWidth;

  private static int LeftAdjustment;
  private static int RightAdjustment;

  // The widget's Interaction must have been set appropriately in order to use these without error.
  protected bool IsHovering => (this.Interaction as Interaction.Clickable)!.IsHovering;
  protected bool IsDragging => (this.Interaction as Interaction.Draggable)!.IsDragging;
  protected Point MousePosition => (this.Interaction as Interaction.Draggable)!.MousePosition;

  private readonly string Name;
  private readonly string? Tooltip;
  private readonly Interaction? Interaction;
  private readonly Alignment? Alignment;

  private int Width;
  private int Height;

  protected Widget(
      string? name = null,
      string? tooltip = null,
      Interaction? interaction = null,
      Alignment? alignment = null) {

    this.Name = name ?? string.Empty;
    this.Tooltip = tooltip;
    this.Interaction = interaction;
    this.Alignment = alignment;
  }

  protected Widget(string? name, string? tooltip, Alignment? alignment)
      : this(name, tooltip, interaction: null, alignment) { }

  protected Widget(Interaction interaction)
      : this(name: null, tooltip: null, interaction, alignment: null) { }

  protected Widget(Alignment? alignment)
      : this(name: null, tooltip: null, interaction: null, alignment) { }

  internal void AddToConfigMenu(IGenericModConfigMenuApi gmcmApi, IManifest modManifest) {
    gmcmApi.AddComplexOption(
        mod: modManifest,
        name: () => this.Name,
        draw: (sb, position) => this.Draw(sb, position, null, null),
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

  private void Draw(
      SpriteBatch sb, Vector2 position, int? containerWidth = null, int? containerHeight = null) {
    this.Alignment?.Align(
        ref position, this.Width, this.Height,
        containerWidth ?? TotalWidth, containerHeight ?? DefaultHeight,
        LeftAdjustment, RightAdjustment);
    this.Interaction?.Update(new((int) position.X, (int) position.Y, this.Width, this.Height));
    this.Draw(sb, position);
  }

  private void OnMenuOpening() {
    if (ViewportWidth != Game1.uiViewport.Width) {
      ViewportWidth = Game1.uiViewport.Width;
      TotalWidth = Math.Min(ViewportWidth - ViewportPadding, MinTotalWidth);
      LeftAdjustment = (TotalWidth / 2) + 8;
      RightAdjustment = (TotalWidth / 2) - 8;
    }
    this.RefreshStateAndSize();
  }

  private void RefreshStateAndSize() {
    this.ResetState();
    (this.Width, this.Height) = this.UpdateDimensions(TotalWidth);
  }
}
