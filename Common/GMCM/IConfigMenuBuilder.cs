namespace Nuztalgia.StardewMods.Common;

internal interface IConfigMenuBuilder {

  IConfigMenuBuilder CreateNewPage(
      out IConfigPageBuilder pageBuilder,
      string pageId,
      string? pageTitle = null);

  void PublishMenu(
      Action? onReset = null,
      Action? onSave = null,
      Action<string, object>? onFieldChanged = null);
}
