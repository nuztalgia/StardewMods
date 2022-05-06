using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.LazyComms;

internal class ModEntry : BaseMod {

  protected override void OnModEntry() {
    Aliases.Initialize(this.Helper.ModContent);
    HarmonyHelper.Patch(this.ModManifest.UniqueID);
  }
}
