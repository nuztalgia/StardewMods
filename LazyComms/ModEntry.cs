using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;

namespace Nuztalgia.StardewMods.LazyComms;

internal class ModEntry : BaseMod {

  protected override void OnModEntry() {
    I18n.Init(this.Helper.Translation);
    HarmonyHelper.Patch(this.ModManifest.UniqueID);
  }

  protected override void OnGameLaunched() {
    this.TryIntegrateWithGMCM(out Integration? gmcm);
    ConfigHelper.Initialize(this.Helper, gmcm);
  }
}
