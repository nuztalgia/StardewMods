using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class FlowerQueensCrown : BaseCompatSection {

  internal enum FlowerQueenChoice {
    Random,
    Everyone
  }

  private const string ModId = "DSVTeam.DiverseSeasonalOutfits.FlowerQueensCrown";
  private const string ModName = "Flower Queen's Crown";

  public FlowerQueenChoice FlowerQueen { get; set; } = FlowerQueenChoice.Random;
  public bool TownspeopleOnly { get; set; } = false;

  internal FlowerQueensCrown() : base(ModId, ModName) { }

  protected override string? GetTooltip(PropertyInfo property) {
    return (property.PropertyType == typeof(FlowerQueenChoice))
            ? I18n.Tooltip_FlowerQueen()
            : I18n.Tooltip_TownspeopleOnly();
  }
}
