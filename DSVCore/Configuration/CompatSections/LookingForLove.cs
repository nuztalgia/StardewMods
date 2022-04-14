using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class LookingForLove : BaseCompatSection {

  private const string ModId = "foggy.LfL";
  private const string ModName = "Looking For Love";
  private const string TokenName = "LookingForLove";

  public bool Clint { get; set; } = true;
  public bool Gus { get; set; } = true;
  public bool Lewis { get; set; } = true;
  public bool Linus { get; set; } = true;
  public bool Marnie { get; set; } = true;
  public bool Pam { get; set; } = true;
  public bool Sandy { get; set; } = true;
  public bool Willy { get; set; } = true;
  public bool Wizard { get; set; } = true;

  internal LookingForLove() : base(ModId, ModName) { }

  internal override void RegisterTokens() {
    if (this.IsAvailable()) {
      // TODO: See if we can get this token directly from LookingForLove's own mod config.
      TokenRegistry.AddCompositeToken(TokenName, new() {
        ["Clint"] = () => this.Clint,
        ["Gus"] = () => this.Gus,
        ["Lewis"] = () => this.Lewis,
        ["Linus"] = () => this.Linus,
        ["Marnie"] = () => this.Marnie,
        ["Pam"] = () => this.Pam,
        ["Sandy"] = () => this.Sandy,
        ["Willy"] = () => this.Willy,
        ["Wizard"] = () => this.Wizard
      });
    } else {
      RegisterDummyTokens(TokenName);
    }
  }

  protected override string? GetOptionName(PropertyInfo property) {
    return string.Format(I18n.Option_LookingForLove_DateableNpc(), property.Name);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return string.Format(I18n.Tooltip_LookingForLove_DateableNpc(), property.Name);
  }
}