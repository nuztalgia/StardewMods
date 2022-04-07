using ContentPatcher;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal class ContentPatcherIntegration : BaseIntegration<IContentPatcherAPI> {

  internal ContentPatcherIntegration() : base(Globals.ModRegistry, "Pathoschild.ContentPatcher") { }

  // Returns true if all tokens were registered successfully.
  internal bool RegisterTokens() {
    if (this.Api is null) {
      Log.Error("Could not retrieve the Content Patcher API. This mod will not function at all.");
      return false;
    }
    // TODO: Actually register tokens.
    return true;
  }
}
