using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCharacterSection : BaseMenuSection {

  private const string VariantTokenName = "Variant";
  private const string ImmersionTokenName = "LightweightConfig";

  internal enum StandardVariant {
    Modded,
    Vanilla,
    Off
  }

  internal enum StandardImmersion {
    Full,
    Light,
    Ultralight
  }

  internal enum SimpleImmersion {
    Full,
    Light
  }

  // Subclasses should override this method if they have any non-standard tokens.
  internal override void RegisterTokens() {
    if (this.TryGetTokenProperty(propertyName: "Variant", out PropertyInfo? variantProperty)) {
      this.RegisterVariantToken<StandardVariant>(() => variantProperty.GetValue(this));
    }
    this.TryRegisterStandardImmersionTokenUsingReflection();
  }

  internal override string GetDisplayName() {
    // To keep things simple, all characters are defined in classes that match their actual names.
    return this.Name;
  }

  internal override bool IsAvailable() {
    // This is only checked if the character's content pack is loaded, so they're always available.
    return true;
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return property.PropertyType.Name switch {
      nameof(StandardImmersion) => this.FormatCharacterString(I18n.Tooltip_Immersion_Standard),
      nameof(SimpleImmersion) => this.FormatCharacterString(I18n.Tooltip_Immersion_Simple),
      _ => base.GetTooltip(property)
    };
  }

  protected string FormatCharacterString(Func<string> getString) {
    return string.Format(getString(), this.GetDisplayName());
  }

  protected void RegisterAutoNamedBoolToken(string tokenName, Func<bool> getValue) {
    TokenRegistry.AddBoolToken(this.Name + tokenName, getValue, autoValueString: tokenName);
  }

  protected void RegisterVariantToken<T>(Func<object?> getValue) where T : Enum {
    string enumName = typeof(T).Name;
    if (enumName.EndsWith(VariantTokenName)) {
      TokenRegistry.AddEnumToken<T>(this.Name + VariantTokenName, getValue);
    } else {
      Log.Error($"Enum named '{enumName}' doesn't end with '{VariantTokenName}'. That's " +
                $"confusing. Will not register '{VariantTokenName}' token for '{this.Name}'.");
    }
  }

  protected void RegisterImmersionToken<T>(Func<object?> getValue) where T : Enum {
    Type enumType = typeof(T);
    if ((enumType == typeof(StandardImmersion)) || (enumType == typeof(SimpleImmersion))) {
      TokenRegistry.AddEnumToken<T>(this.Name + ImmersionTokenName, getValue);
    } else {
      Log.Error($"Enum named '{enumType.Name}' is not a valid immersion type. " +
                $"Will not register '{ImmersionTokenName}' token for '{this.Name}'.");
    }
  }

  // Use the above method instead of this one when possible, because reflection is slow.
  protected void TryRegisterStandardImmersionTokenUsingReflection() {
    if (this.TryGetTokenProperty(propertyName: "Immersion", out PropertyInfo? property)) {
      this.RegisterImmersionToken<StandardImmersion>(() => property.GetValue(this));
    }
  }

  protected bool TryGetTokenProperty(
      string propertyName, [NotNullWhen(true)] out PropertyInfo? property) {
    property = this.GetType().GetProperty(propertyName);
    if (property is not null) {
      return true;
    } else {
      Log.Error($"Class named '{this.Name}' does not have a property named '{propertyName}'. " +
                $"Will not register custom token named '{this.Name}{propertyName}'.");
      return false;
    }
  }
}

// Extension methods for the enums defined at the top of BaseCharacterSection (above).
internal static class CharacterEnumExtensions {
  internal static bool IsModded(this BaseCharacterSection.StandardVariant variant) {
    return variant != BaseCharacterSection.StandardVariant.Modded;
  }

  internal static bool IsNotUltralight(this BaseCharacterSection.StandardImmersion immersion) {
    return immersion != BaseCharacterSection.StandardImmersion.Ultralight;
  }

  internal static bool IsFull(this BaseCharacterSection.StandardImmersion immersion) {
    return immersion == BaseCharacterSection.StandardImmersion.Full;
  }

  internal static bool IsFull(this BaseCharacterSection.SimpleImmersion immersion) {
    return immersion == BaseCharacterSection.SimpleImmersion.Full;
  }
}