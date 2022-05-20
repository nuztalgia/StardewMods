using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ContentPatcher;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.Common;

internal abstract class BaseMod : Mod {

  public override sealed void Entry(IModHelper helper) {
    Log.Initialize(this.Monitor);

#if MOD_REGISTRY
    ModRegistryUtils.Initialize(helper.ModRegistry);
#endif

    this.OnModEntry();

    helper.Events.GameLoop.GameLaunched +=
        (object? sender, GameLaunchedEventArgs e) => this.OnGameLaunched();
  }

  protected virtual void OnModEntry() { }

  protected virtual void OnGameLaunched() { }

#if CONTENT_PATCHER
  protected bool TryIntegrateWithCP(
      [NotNullWhen(true)] out ContentPatcherIntegration? integration) {
    Log.Trace("Initializing Content Patcher integration.");
    return this.TryIntegrate<IContentPatcherAPI, ContentPatcherIntegration>(
        "Pathoschild.ContentPatcher", out integration);
  }
#endif

#if GMCM
  protected bool TryIntegrateWithGMCM(
      [NotNullWhen(true)] out GenericModConfigMenuIntegration? integration) {
    Log.Trace("Initializing Generic Mod Config Menu integration.");
    return this.TryIntegrate<IGenericModConfigMenuApi, GenericModConfigMenuIntegration>(
        "spacechase0.GenericModConfigMenu", out integration);
  }
#endif

  private bool TryIntegrate<TInterface, TInteg>(string modId, [NotNullWhen(true)] out TInteg? integ)
      where TInterface : class
      where TInteg : BaseIntegration<TInterface> {

    TInterface? api = this.Helper.ModRegistry.GetApi<TInterface>(modId);
    integ = (api is not null) ? TryConstructInteg() : null;

    return integ is not null;

    TInteg? TryConstructInteg() {
      object[] constructorArgs = new object[] { api, this.ModManifest };
      return (TInteg?) typeof(TInteg).GetConstructor(
          bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
          binder: null, // Use default binder.
          types: Type.GetTypeArray(constructorArgs),
          modifiers: null
      )?.Invoke(constructorArgs);
    }
  }
}
