using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseMenuComponent {

  internal readonly string Name;

  protected BaseMenuComponent() {
    this.Name = this.GetType().Name;
  }

  internal abstract void RegisterTokens(Integration contentPatcher);

  internal abstract string GetDisplayName();

  internal abstract bool IsAvailable();
}
