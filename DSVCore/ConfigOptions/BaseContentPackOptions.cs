using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nuztalgia.StardewMods.Common;
using System;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackOptions {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";
  private const string PropertyClassSuffix = "Options";

  private static readonly JsonSerializerSettings JsonSettings = new() {
    Formatting = Formatting.Indented,
    Converters = new JsonConverter[] { new StringEnumConverter() }
  };

  private readonly string ContentPackName;
  private readonly string ContentPackId;
  private readonly Lazy<string?> DisplayName;

  internal BaseContentPackOptions() {
    this.ContentPackName = this.GetType().Name.Replace(PropertyClassSuffix, "");
    this.ContentPackId = $"{RootModId}.{this.ContentPackName}";

    Log.Verbose($"Ready to handle content pack: {this.ContentPackId}");

    this.DisplayName = new(() => {
      FieldInfo? fieldInfo = typeof(I18n.Keys).GetField($"Pack_{this.ContentPackName}_Name");
      return (fieldInfo?.GetValue(null) is string key) ? I18n.GetByKey(key) : null;
    });
  }

  public override string ToString() {
    return JsonConvert.SerializeObject(this, JsonSettings);
  }

  internal string GetDisplayName() {
    return (this.DisplayName.Value is not null)
        ? this.DisplayName.Value
        : throw new InvalidOperationException($"No display name for {this.ContentPackName}.");
  }

  internal string GetPageId() {
    return this.ContentPackName;
  }

  internal bool IsContentPackLoaded() {
    return Globals.ModRegistry.IsLoaded(this.ContentPackId);
  }
}
