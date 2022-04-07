using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseOptions {

  private static readonly JsonSerializerSettings JsonSettings = new() {
    Formatting = Formatting.Indented,
    Converters = new JsonConverter[] { new StringEnumConverter() }
  };

  internal readonly string Name;

  protected BaseOptions() {
    this.Name = this.GetType().Name.Replace("Options", "");
  }

  public override string ToString() {
    return JsonConvert.SerializeObject(this, JsonSettings);
  }
}
