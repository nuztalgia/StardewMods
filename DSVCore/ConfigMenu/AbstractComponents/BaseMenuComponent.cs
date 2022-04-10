using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

  internal abstract string GetDisplayName();

  internal abstract bool IsAvailable();
}
