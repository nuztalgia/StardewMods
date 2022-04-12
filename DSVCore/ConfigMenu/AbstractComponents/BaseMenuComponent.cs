using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseMenuComponent {

  private static readonly JsonSerializerSettings JsonSettings = new() {
    Formatting = Formatting.Indented,
    Converters = new JsonConverter[] { new StringEnumConverter() }
  };

  internal readonly string Name;

  protected BaseMenuComponent() {
    this.Name = this.GetType().Name;
  }

  public override string ToString() {
    return JsonConvert.SerializeObject(this, JsonSettings);
  }

  protected static IEnumerable<string> WrapTokenValue(
      object? value, string? valueIfTrue = null, string? valueIfFalse = null) {
    if ((value is int intValue) && (intValue < 0)) {
      value = null; // Negativity is unacceptable.
    }
    if (value is bool boolValue) {
      value = boolValue ? (valueIfTrue ?? value) : (valueIfFalse ?? value);
    }
    return ((value?.ToString() is string stringValue) && (stringValue.Length > 0))
            ? new[] { stringValue }
            : Array.Empty<string>();
  }

  protected void AddTokenByProperty(Dictionary<string, Func<IEnumerable<string>>> tokenMap,
      string propertyName, string? customPrefix = null, string? customSuffix = null,
      string? valueIfTrue = null, string? valueIfFalse = null) {
    if (this.GetType().GetProperty(propertyName) is PropertyInfo property) {
      string tokenName = (customPrefix ?? this.Name) + (customSuffix ?? propertyName);
      tokenMap.Add(tokenName,
                   () => WrapTokenValue(property.GetValue(this), valueIfTrue, valueIfFalse));
    }
  }

  protected IEnumerable<string> GetCombinedTokenValues(params string[] propertyNames) {
    Type type = this.GetType();
    IEnumerable<PropertyInfo> properties = propertyNames.Any()
                                           ? propertyNames.Select(name => type.GetProperty(name)!)
                                           : type.GetProperties();
    foreach (PropertyInfo property in properties) {
      if ((property.GetValue(this) is bool boolValue) && boolValue) {
        yield return property.Name;
      }
    }
  }

  internal abstract void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap);

  internal abstract string GetDisplayName();

  internal abstract bool IsAvailable();
}
