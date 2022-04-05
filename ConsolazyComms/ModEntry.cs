using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.ConsolazyComms;

public class ModEntry : Mod {

  public override void Entry(IModHelper helper) {
    Log.Initialize(this.Monitor);
    // TODO: Implement stuff.
  }
}
