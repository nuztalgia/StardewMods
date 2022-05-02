using System.Collections.Generic;
using System.Linq;
using Nuztalgia.StardewMods.Common;

using CPIntegration = Nuztalgia.StardewMods.Common.ContentPatcher.Integration;
using GMCMIntegration = Nuztalgia.StardewMods.Common.GenericModConfigMenu.Integration;

namespace Nuztalgia.StardewMods.DSVCore;

internal class ModEntry : BaseMod {

  private ModConfig ModConfig = new();

  protected override void OnModEntry() {
    I18n.Init(this.Helper.Translation);
  }

  protected override void OnGameLaunched() {
    if (!this.TryIntegrateWithCP(out CPIntegration? cpIntegration)) {
      Log.Error("Could not retrieve the Content Patcher API. This mod will not function at all.");
      return;
    }

    this.ModConfig = this.Helper.ReadConfig<ModConfig>();

    IEnumerable<BaseMenuPage> allMenuPages =
        this.ModConfig.GetType().GetProperties()
            .Select(property => property.GetValue(this.ModConfig))
            .OfType<BaseMenuPage>();

    foreach (BaseMenuPage menuPage in allMenuPages) {
      menuPage.RegisterTokens(cpIntegration);
    }

    if (!this.TryIntegrateWithGMCM(out GMCMIntegration? gmcmIntegration)) {
      Log.Warn(
          "Could not retrieve the Generic Mod Config Menu API. " +
          "A significant portion of the functionality of this mod will be unavailable.");
      return;
    }

    gmcmIntegration.Register(
        resetAction: () => this.ModConfig = new ModConfig(),
        saveAction: () => this.Helper.WriteConfig(this.ModConfig)
    );

    ConfigMenuHelper configMenuHelper = new(gmcmIntegration, this.Helper.GameContent);

    configMenuHelper.SetUpMenuPages(
        coreAndCompatPage: this.ModConfig.CoreAndCompat,
        installedContentPackPages: GetContentPackPages(installed: true),
        otherContentPackPages: GetContentPackPages(installed: false)
    );

    IEnumerable<BaseContentPackPage> GetContentPackPages(bool installed) {
      return allMenuPages.OfType<BaseContentPackPage>().Where(
          contentPackPage => contentPackPage.IsAvailable() == installed);
    }
  }
}
