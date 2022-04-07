using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackOptions {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";
  private const string PropertyClassSuffix = "Options";

  private static JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings {
    Formatting = Formatting.Indented,
    Converters = new JsonConverter[] { new StringEnumConverter() }
  };

  private readonly string ContentPackName;
  private readonly string ContentPackId;

  internal BaseContentPackOptions() {
    this.ContentPackName = this.GetType().Name.Replace(PropertyClassSuffix, "");
    this.ContentPackId = $"{RootModId}.{this.ContentPackName}";
  }

  public override string ToString() {
    return JsonConvert.SerializeObject(this, JsonSettings);
  }

  internal string GetDisplayName() {
    FieldInfo? fieldInfo = typeof(I18n.Keys).GetField($"Pack_{this.ContentPackName}_Name");
    return (fieldInfo?.GetValue(null) is string key) ? I18n.GetByKey(key) : "???";
  }

  internal bool IsContentPackLoaded() {
    return Globals.ModRegistry.IsLoaded(this.ContentPackId);
  }
}
