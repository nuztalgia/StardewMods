using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common;

internal abstract class BaseIntegration<TInterface> where TInterface : class {

  protected TInterface Api { get; private set; }
  protected IManifest Manifest { get; private set; }

  protected BaseIntegration(TInterface api, IManifest manifest) {
    this.Api = api;
    this.Manifest = manifest;
  }
}
