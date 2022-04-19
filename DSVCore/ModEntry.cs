using System;
using System.Collections.Generic;
using System.Linq;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal class ModEntry : BaseMod {

  private ModConfig ModConfig = new();

  protected override void OnModEntry() {
    Globals.Init(this.Helper.ModRegistry);
    I18n.Init(this.Helper.Translation);
  }

  protected override void OnGameLaunched() {
    if (!this.TryIntegrateWithCP(out ContentPatcherIntegration? cpIntegration)) {
      Log.Error("Could not retrieve the Content Patcher API. This mod will not function at all.");
      return;
    }

    this.ModConfig = this.Helper.ReadConfig<ModConfig>();

    IEnumerable<BaseMenuPage> allMenuPages =
        this.ModConfig.GetType().GetProperties()
            .Select(property => property.GetValue(this.ModConfig))
            .OfType<BaseMenuPage>();

    foreach (BaseMenuPage menuPage in allMenuPages) {
      menuPage.RegisterTokens();
    }

    foreach (KeyValuePair<string, Func<IEnumerable<string>>> token in TokenRegistry.GetData()) {
      cpIntegration.RegisterToken(token.Key, token.Value);
    }

    if (!this.TryIntegrateWithGMCM(out GenericModConfigMenuIntegration? gmcmIntegration)) {
      Log.Warn("Could not retrieve the Generic Mod Config Menu API. " +
               "A lot of the functionality of this mod will be disabled.");
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
