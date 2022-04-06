using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common;

internal abstract class BaseIntegration<TInterface> where TInterface : class {

  protected string ModId { get; }
  protected TInterface? Api { get; }

  protected BaseIntegration(IModRegistry modRegistry, string modId) {
    this.ModId = modId;
    this.Api = modRegistry.GetApi<TInterface>(modId);
  }
}
