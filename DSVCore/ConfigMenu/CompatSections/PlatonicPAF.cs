using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class PlatonicPAF : BaseCompatSection {

  private const string ModId = "Amaranthacyan.PlatonicPartnersandFriendships";
  private const string ModName = "Platonic Partners and Friendships";

  public bool Abigail { get; set; } = true;
  public bool Alex { get; set; } = true;
  public bool Elliott { get; set; } = true;
  public bool Emily { get; set; } = true;
  public bool Haley { get; set; } = true;
  public bool Harvey { get; set; } = true;
  public bool Leah { get; set; } = true;
  public bool Maru { get; set; } = true;
  public bool Penny { get; set; } = true;
  public bool Sam { get; set; } = true;
  public bool Sebastian { get; set; } = true;
  public bool Shane { get; set; } = true;

  internal PlatonicPAF() : base(ModId, ModName) { }

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    tokenMap.Add("PlatonicNPCs", () => this.GetCombinedTokenValues());
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return string.Format(I18n.Option_PlatonicPartnersAndFriendships(), property.Name);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return string.Format(I18n.Tooltip_PlatonicPartnersAndFriendships(), property.Name);
  }
}
