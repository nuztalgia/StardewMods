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
      bool contentPatcherSuccess = new ContentPatcherIntegration().RegisterTokens();
      if (contentPatcherSuccess) {
        new GenericModConfigMenuIntegration().SetUpConfigMenu(helper.WriteConfig);
      } else {
        Log.Warn("Something went wrong while registering CP tokens. Skipping GMCM setup.");
      }
    };
  }
}
