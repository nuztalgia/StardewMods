using System.Diagnostics.CodeAnalysis;
using Nuztalgia.StardewMods.Common.UI;

namespace Nuztalgia.StardewMods.Common;

internal sealed class GenericModConfigMenuIntegration : BaseIntegration<IGenericModConfigMenuApi> {

  private delegate void CustomLog(Action<string> log, string message);

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private GenericModConfigMenuIntegration(IGenericModConfigMenuApi api, IManifest manifest)
      : base(api, manifest) { }

  [SuppressMessage("Style", "IDE0039:Use local function", Justification =
      "The inlined log actions are very simple, and inlining them is more compact and readable.")]
  internal IConfigMenuBuilder CreateMenuBuilder(
      out IConfigPageBuilder mainPageBuilder, IManifest? modManifest = null) {

    Widget.MenuPage? mainMenuPageWidget = null;
    IConfigPageBuilder? mainMenuPageBuilder = null;
    IManifest manifest = modManifest ?? this.Manifest;

    CustomLog customLog =
        (manifest.UniqueID == this.Manifest.UniqueID)
            ? (log, message) => log(message)
            : (log, message) => log($"[{manifest.UniqueID}] {message}");

    Action<string> logWarning = (message) => customLog(Log.Warn, message);
    Action<string> logTrace = (message) => customLog(Log.Trace, message);
    Action<string> logVerbose = (message) => customLog(Log.Verbose, message);

    IConfigMenuBuilder menuBuilder = new ConfigMenuBuilder(
        MenuId: manifest.UniqueID,
        Register: RegisterMenu,
        AddPage: AddPageToMenu,
        LogWarning: logWarning,
        LogTrace: logTrace,
        LogVerbose: logVerbose);

    mainMenuPageBuilder = new ConfigPageBuilder(
        PageId: manifest.Name,
        End: EndMainMenuPage,
        LogWarning: logWarning,
        LogVerbose: logVerbose);

    mainPageBuilder = mainMenuPageBuilder;
    return menuBuilder;

    void RegisterMenu(Action onReset, Action onSave) {
      if (mainMenuPageWidget == null) {
        mainMenuPageBuilder!.EndPage();
      }
      this.Api.Register(manifest, onReset, onSave);
      mainMenuPageWidget!.AddToConfigMenu(this.Api, this.Manifest);
    }

    void AddPageToMenu(string pageId, string pageTitle, Widget.MenuPage menuPageWidget) {
      this.Api.AddPage(manifest, pageId, () => pageTitle);
      menuPageWidget.AddToConfigMenu(this.Api, this.Manifest);
    }

    IConfigMenuBuilder EndMainMenuPage(Widget.MenuPage? menuPageWidget) {
      if (menuPageWidget == null) {
        logWarning("Tried to end the main menu page, but it has already been ended.");
      } else {
        logVerbose("Ending the main menu page and adding it to the in-progress config menu.");
        mainMenuPageWidget = menuPageWidget;
      }
      return menuBuilder;
    }
  }

  internal void OpenMenuForMod(IManifest modManifest) {
    this.Api.OpenModMenu(modManifest);
  }
}
