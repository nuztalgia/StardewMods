using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Nuztalgia.StardewMods.DSVCore;

public class ModEntry : Mod {

  public override void Entry(IModHelper helper) {
    Log.Initialize(this.Monitor);
    Globals.Initialize(this.ModManifest, helper);
    I18n.Init(helper.Translation);

    helper.Events.GameLoop.GameLaunched += (object? sender, GameLaunchedEventArgs e) => {
      new MenuRegistry(helper.ModRegistry, this.ModManifest).InitializeMenus();
      new TokenRegistry(helper.ModRegistry, this.ModManifest).InitializeTokens();
    };
  }
}
