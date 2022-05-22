namespace Nuztalgia.StardewMods.Common;

internal interface IConfigMenuBuilder {

  void PublishMenu(Action? onReset = null, Action? onSave = null);

  IConfigMenuBuilder CreateNewPage(
      out IConfigPageBuilder pageBuilder,
      string pageId,
      string? pageTitle = null);
}
