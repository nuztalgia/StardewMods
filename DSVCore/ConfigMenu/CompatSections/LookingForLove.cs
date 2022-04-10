using System.Reflection;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class LookingForLove : BaseCompatSection {

  private const string ModId = "foggy.LfL";
  private const string ModName = "Looking For Love";

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

  protected override string? GetOptionName(PropertyInfo property) {
    return string.Format(I18n.Option_LookingForLove(), property.Name);
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return string.Format(I18n.Tooltip_LookingForLove(), property.Name);
  }
}
