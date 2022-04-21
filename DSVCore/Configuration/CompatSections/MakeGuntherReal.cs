using System.Collections.Generic;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class MakeGuntherReal : BaseCompatSection {

  private const string ModId = "Wolf.Gun";
  private const string ModName = "Make Gunther Real";
  private const string TokenName = "AlternateCecily";

  public bool AlternateCecily { get; set; } = true;

  internal MakeGuntherReal() : base(ModId, ModName) { }

  protected override void RegisterAllTokens(ContentPatcherIntegration contentPatcher) {
    contentPatcher.RegisterBoolToken(TokenName, () => this.AlternateCecily);
  }

  protected override IEnumerable<string> GetTokenNames() {
    yield return TokenName;
  }
}
