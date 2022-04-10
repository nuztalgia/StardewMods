namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseCompatSection : BaseMenuSection {

  private readonly string ModId;
  private readonly string ModName;

  protected BaseCompatSection(string modId, string modName) {
    this.ModId = modId;
    this.ModName = modName;
  }

  internal override string GetDisplayName() {
    return this.ModName;
  }

  internal override bool IsAvailable() {
    return Globals.ModRegistry.IsLoaded(this.ModId);
  }
}
