using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Nuztalgia.StardewMods.Common;

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
  }

  private static readonly Rectangle[][] StandardPortraitRect = Wrap(new Rectangle(0, 0, 64, 64));
  private static readonly Rectangle[][] StandardSpriteRect = Wrap(new Rectangle(0, 0, 16, 32));

  internal override sealed void RegisterTokens(ContentPatcherIntegration contentPatcher) {
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

  internal override sealed (int min, int max) GetValueRange(PropertyInfo property) {
    return (property.Name == nameof(IHasWeddingOutfit.WeddingOutfit))
        ? (this as IHasWeddingOutfit)?.GetWeddingOutfitValueRange()
            ?? throw new ArgumentException($"WeddingOutfit property is not valid for {this.Name}.")
        : base.GetValueRange(property);
  }

  internal virtual string[][] GetGameImagePaths(string imageDirectory) {
    return Wrap($"{imageDirectory}/{this.Name}");
  }

  internal virtual string[][] GetModImagePaths(
      string imageDirectory, IDictionary<string, object?> ephemeralTraits) {
    return ((this is IHasCustomModImagePath or IHasVariant)
            && ephemeralTraits.TryGetValue(nameof(IHasVariant<Enum>.Variant), out object? value)
            && (value?.ToString() is string variant) && (variant != nameof(StandardVariant.Off)))
        ? Wrap((this as IHasCustomModImagePath)?.GetModImagePath(imageDirectory)
            ?? this.GetModImagePath(
                imageDirectory, variant, (this as IHasVariant)!.GetPreviewOutfit()))
        : Wrap<string>();
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
  protected virtual void RegisterExtraTokens(ContentPatcherIntegration contentPatcher) { }

  protected static T[][] Wrap<T>(params T[] items) {
    return new T[][] { items };
  }

  private string GetModImagePath(string imageDirectory, string variant, string outfit) {
    StringBuilder path = new($"{this.Name}/{imageDirectory}/");
    path.Append((this as IHasCustomModImageDirectory)?.GetDirectory(variant) ?? variant);
    return path.Append($"/{this.Name}_{outfit}.png").ToString();
  }

  private string FormatCharacterDisplayString(string? displayString) {
    return (displayString ?? string.Empty).Format(this.GetDisplayName());
  }
}
