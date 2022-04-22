using System.Collections.Generic;
using Nuztalgia.StardewMods.Common.ContentPatcher;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class FlowerQueensCrown : BaseCompatSection {

  internal enum FlowerQueenChoice {
    Random,
    Everyone
  }

  private const string ModId = "DSVTeam.DiverseSeasonalOutfits.FlowerQueensCrown";
  private const string ModName = "Flower Queen's Crown";
  private const string FlowerQueenTokenName = "FlowerQueensCrown";
  private const string TownspeopleTokenName = "TownspeopleOnly";

  public FlowerQueenChoice FlowerQueen { get; set; } = FlowerQueenChoice.Random;
  public bool TownspeopleOnly { get; set; } = false;

  internal FlowerQueensCrown() : base(ModId, ModName) { }

  protected override void RegisterAllTokens(Integration contentPatcher) {
    contentPatcher.RegisterEnumToken(FlowerQueenTokenName, () => this.FlowerQueen);
    contentPatcher.RegisterBoolToken(TownspeopleTokenName, () => this.TownspeopleOnly);
  }

  protected override IEnumerable<string> GetTokenNames() {
    yield return FlowerQueenTokenName;
    yield return TownspeopleTokenName;
  }
}
