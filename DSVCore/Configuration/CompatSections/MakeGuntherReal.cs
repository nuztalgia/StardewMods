using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class MakeGuntherReal : BaseCompatSection {

  private const string ModId = "Wolf.Gun";
  private const string ModName = "Make Gunther Real";
  private const string TokenName = "AlternateCecily";

  public bool AlternateCecily { get; set; } = true;

  internal MakeGuntherReal() : base(ModId, ModName) { }

  internal override void RegisterTokens(ContentPatcherIntegration contentPatcher) {
    if (this.IsAvailable()) {
      contentPatcher.RegisterBoolToken(TokenName, () => this.AlternateCecily);
    } else {
      RegisterDummyTokens(contentPatcher, TokenName);
    }
  }
}
