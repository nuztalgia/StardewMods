namespace Nuztalgia.StardewMods.LazyComms;

internal class ModEntry : BaseMod {

  protected override void OnModEntry() {
    PatchHelper.ApplyPatches(this.ModManifest.UniqueID);
  }

  protected override void OnGameLaunched() {
    this.TryIntegrateWithGMCM(out GenericModConfigMenuIntegration? gmcm);
    ConfigHelper.Initialize(this.Helper, gmcm);
  }
}
