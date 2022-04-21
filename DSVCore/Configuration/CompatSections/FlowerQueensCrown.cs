using Nuztalgia.StardewMods.Common;

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

  internal override void RegisterTokens(ContentPatcherIntegration contentPatcher) {
    if (this.IsAvailable()) {
      contentPatcher.RegisterEnumToken(FlowerQueenTokenName, () => this.FlowerQueen);
      contentPatcher.RegisterBoolToken(TownspeopleTokenName, () => this.TownspeopleOnly);
    } else {
      RegisterDummyTokens(contentPatcher, FlowerQueenTokenName, TownspeopleTokenName);
    }
  }
}
