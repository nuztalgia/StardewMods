namespace Nuztalgia.StardewMods.Common;

internal abstract class BaseIntegration<TInterface> where TInterface : class {

  internal TInterface Api { get; private init; }
  internal IManifest Manifest { get; private init; }

  protected BaseIntegration(TInterface api, IManifest manifest) {
    this.Api = api;
    this.Manifest = manifest;
  }
}
