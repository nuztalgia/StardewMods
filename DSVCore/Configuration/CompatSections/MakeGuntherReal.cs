using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class MakeGuntherReal : BaseCompatSection {

  private const string TokenName = "AlternateCecily";

  public bool AlternateCecily { get; set; } = true;

  internal MakeGuntherReal() : base(
      modId: "Wolf.Gun",
      modName: "Make Gunther Real",
      tokenNames: new string[] { TokenName }
  ) { }

  protected override void RegisterCompatTokens(Integration contentPatcher) {
    contentPatcher.RegisterBoolToken(TokenName, () => this.AlternateCecily);
  }
}
