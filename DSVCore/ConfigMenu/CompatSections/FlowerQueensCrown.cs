using System.Reflection;

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

  internal override void RegisterTokens() {
    if (this.IsAvailable()) {
      TokenRegistry.AddEnumToken<FlowerQueenChoice>(FlowerQueenTokenName, () => this.FlowerQueen);
      TokenRegistry.AddBoolToken(TownspeopleTokenName, () => this.TownspeopleOnly);
    } else {
      RegisterDummyTokens(FlowerQueenTokenName, TownspeopleTokenName);
    }
  }
}
