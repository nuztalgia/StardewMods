using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class FlowerQueensCrown : BaseCompatSection {

  internal enum FlowerQueenChoice {
    Random,
    Everyone
  }

  private const string FlowerQueenTokenName = "FlowerQueensCrown";
  private const string TownspeopleTokenName = "TownspeopleOnly";

  public FlowerQueenChoice FlowerQueen { get; set; } = FlowerQueenChoice.Random;
  public bool TownspeopleOnly { get; set; } = false;

  internal FlowerQueensCrown() : base(
      modId: "DSVTeam.DiverseSeasonalOutfits.FlowerQueensCrown",
      modName: "Flower Queen's Crown",
      tokenNames: new string[] { FlowerQueenTokenName, TownspeopleTokenName }
  ) { }

  protected override void RegisterCompatTokens(Integration contentPatcher) {
    contentPatcher.RegisterEnumToken(FlowerQueenTokenName, () => this.FlowerQueen);
    contentPatcher.RegisterBoolToken(TownspeopleTokenName, () => this.TownspeopleOnly);
  }
}
