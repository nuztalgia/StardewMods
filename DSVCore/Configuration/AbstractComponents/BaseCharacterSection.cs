using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.ContentPatcher;
using Nuztalgia.StardewMods.Common.ModRegistry;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCharacterSection : BaseMenuSection {

  // A subclass of BaseCharacterSection used by the majority of supported characters.
  internal abstract class Villager<TVariant> : BaseCharacterSection,
      IHasVariant<TVariant>, IHasImmersion<StandardImmersion> where TVariant : Enum {

    // If a character doesn't have a variant named "Vanilla", then they should override this property.
    public virtual TVariant Variant { get; set; } = (TVariant)
        (Enum.TryParse(typeof(TVariant), "Vanilla", out object? value) ? value : default(TVariant))!;

    public StandardImmersion Immersion { get; set; } = StandardImmersion.Full;

    public abstract string GetPreviewOutfit();
  }

  // A subclass of Villager used by characters that are marriageable in the base game.
  // ("Bachelorex" is a gender-neutral term for "Bachelor" or "Bachelorette".)
  internal abstract class Bachelorex<TVariant> : Villager<TVariant>,
      IHasWeddingOutfit where TVariant : Enum {

    public int WeddingOutfit { get; set; } = IHasWeddingOutfit.FirstWeddingOutfit;

    // TODO: Properly (dynamically?) validate the return value of this method for every bachelorex.
    public abstract int GetNumberOfWeddingOutfits();

    protected bool HasElahoOutfit(string outfitName) {
      return ModRegistry.IsLoaded($"Elaho.{this.Name}{outfitName}");
    }
  }

  private static readonly Rectangle[][] StandardPortraitRect = Wrap(new Rectangle(0, 0, 64, 64));
  private static readonly Rectangle[][] StandardSpriteRect = Wrap(new Rectangle(0, 0, 16, 32));

  internal override sealed void RegisterTokens(Integration contentPatcher) {
    (this as IHasVariant)?.RegisterVariantToken(this.Name, contentPatcher);
    (this as IHasImmersion)?.RegisterImmersionToken(this.Name, contentPatcher);
    (this as IHasWeddingOutfit)?.RegisterWeddingOutfitToken(this.Name, contentPatcher);
    this.RegisterExtraTokens(contentPatcher);
  }

  internal override sealed string GetDisplayName() {
    return (this as IHasCustomDisplayName)?.GetDisplayName() ?? this.Name;
  }

  internal override sealed bool IsAvailable() {
    // This is only checked if the character's content pack is loaded, so they're always available.
    return true;
  }

  internal override sealed IEnumerable<OptionItem> GetOptions() {
    return base.GetOptions().OrderBy(item => item.Property.Name switch {
      nameof(IHasVariant<Enum>.Variant) => 1,
      nameof(IHasImmersion<Enum>.Immersion) => 2,
      nameof(IHasWeddingOutfit.WeddingOutfit) => 3,
      _ => 69 // Other options will be in the order in which they're defined in their own class.
    });
  }

  internal sealed override int GetMinValue(PropertyInfo property) {
    return (property.Name == nameof(IHasWeddingOutfit.WeddingOutfit))
        ? IHasWeddingOutfit.FirstWeddingOutfit
        : base.GetMaxValue(property);
  }

  internal sealed override int GetMaxValue(PropertyInfo property) {
    return (property.Name == nameof(IHasWeddingOutfit.WeddingOutfit))
        ? (this as IHasWeddingOutfit)?.GetNumberOfWeddingOutfits() ?? this.GetMinValue(property)
        : base.GetMaxValue(property);
  }

  internal virtual string[][] GetGameImagePaths(string imageDirectory) {
    return Wrap($"{imageDirectory}/{this.Name}");
  }

  internal virtual string[][] GetModImagePaths(
      string imageDirectory, IDictionary<string, object?> ephemeralProperties) {
    if ((this is IHasCustomModImagePath or IHasVariant)
        && ephemeralProperties.TryGetValue(nameof(IHasVariant<Enum>.Variant), out object? value)
        && (value?.ToString() is string variant) && (variant != nameof(StandardVariant.Off))) {
      string image = (this as IHasCustomModImagePath)?.GetModImagePath(imageDirectory)
          ?? this.GetModImagePath(imageDirectory, variant, ((IHasVariant) this).GetPreviewOutfit());
      IEnumerable<string> overlays =
          this.GetImageOverlayPaths(imageDirectory, variant, ephemeralProperties);
      return overlays.Any() ? new string[][] { overlays.Prepend(image).ToArray() } : Wrap(image);
    } else {
      return Wrap<string>();
    }
  }

  internal virtual ImagePreviewOptions.GetImageRects? GetPortraitRectsDelegate() {
    return _ => StandardPortraitRect;
  }

  internal virtual ImagePreviewOptions.GetImageRects? GetSpriteRectsDelegate() {
    return _ => StandardSpriteRect;
  }

  internal string GetPreviewTooltip() {
    return this.FormatCharacterDisplayString(
        (this as IHasCustomPreviewTooltip)?.GetPreviewTooltip() ?? I18n.Tooltip_Preview_General());
  }

  protected override sealed string? GetTooltip(PropertyInfo property) {
    return this.FormatCharacterDisplayString(property.PropertyType.Name switch {
      nameof(StandardImmersion) => I18n.Tooltip_Immersion_Standard(),
      nameof(SimpleImmersion) => I18n.Tooltip_Immersion_Simple(),
      _ => (property.Name == nameof(IHasWeddingOutfit.WeddingOutfit))
          ? I18n.Tooltip_WeddingOutfit()
          : base.GetTooltip(property)
    });
  }

  // Subclasses should override this method if they have any additional character-specific tokens.
  protected virtual void RegisterExtraTokens(Integration contentPatcher) { }

  // Subclasses should override this method if they have any options that add image overlays.
  protected virtual IEnumerable<string> GetImageOverlayPaths(
      string imageDirectory, string variant, IDictionary<string, object?> ephemeralProperties) {
    return Array.Empty<string>();
  }

  protected static T[][] Wrap<T>(params T[] items) {
    return new T[][] { items };
  }

  protected string GetModImagePath(string imageDirectory, string variant, string? outfit = null) {
    StringBuilder path = new($"{this.Name}/{imageDirectory}/");
    path.Append((this as IHasCustomModImageDirectory)?.GetDirectory(variant) ?? variant);
    path.Append((outfit is not null) ? $"/{this.Name}_{outfit}.png" : "/");
    return path.ToString();
  }

  private string FormatCharacterDisplayString(string? displayString) {
    return (displayString ?? string.Empty).Format(this.GetDisplayName());
  }
}
