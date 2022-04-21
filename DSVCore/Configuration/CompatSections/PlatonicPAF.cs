using System.Reflection;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class PlatonicPAF : BaseCompatSection {

  private const string ModId = "Amaranthacyan.PlatonicPartnersandFriendships";
  private const string ModName = "Platonic Partners and Friendships";
  private const string TokenName = "PlatonicNPCs";

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

  internal override void RegisterTokens(ContentPatcherIntegration contentPatcher) {
    if (this.IsAvailable()) {
      // TODO: See if we can get this token directly from PlatonicPaF's own mod config.
      contentPatcher.RegisterCompositeToken(TokenName, new() {
        ["Abigail"] = () => this.Abigail,
        ["Alex"] = () => this.Alex,
        ["Elliott"] = () => this.Elliott,
        ["Emily"] = () => this.Emily,
        ["Haley"] = () => this.Haley,
        ["Harvey"] = () => this.Harvey,
        ["Leah"] = () => this.Leah,
        ["Maru"] = () => this.Maru,
        ["Penny"] = () => this.Penny,
        ["Sam"] = () => this.Sam,
        ["Sebastian"] = () => this.Sebastian,
        ["Shane"] = () => this.Shane
      });
    } else {
      RegisterDummyTokens(contentPatcher, TokenName);
    }
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return I18n.Option_PlatonicPartnersAndFriendships_PlatonicNpc().Format(property.Name);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return I18n.Tooltip_PlatonicPartnersAndFriendships_PlatonicNpc().Format(property.Name);
  }
}
