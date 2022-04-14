namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal sealed class RidgesideVillage : BaseCompatSection {

  internal enum OnlyModdedVariant {
    Modded,
    Off
  }

  internal enum TrinnieCustomVariant {
    ModdedBlack,
    ModdedGreen,
    Off
  }

  private const string ModId = "Rafseazz.RidgesideVillage";
  private const string ModName = "Ridgeside Village";

  private const string BertTokenName = "RidgesideVillageBert";
  private const string TrinnieTokenName = "RidgesideVillageTrinnie";
  private const string LennyTokenName = "RidgesideVillageLenny";

  public OnlyModdedVariant BertVariant { get; set; } = OnlyModdedVariant.Off;
  public TrinnieCustomVariant TrinnieVariant { get; set; } = TrinnieCustomVariant.Off;
  public OnlyModdedVariant LennyVariant { get; set; } = OnlyModdedVariant.Off;

  internal RidgesideVillage() : base(ModId, ModName) { }

  internal override void RegisterTokens() {
    if (this.IsAvailable()) {
      TokenRegistry.AddEnumToken<OnlyModdedVariant>(BertTokenName, () => this.BertVariant);
      TokenRegistry.AddEnumToken<TrinnieCustomVariant>(TrinnieTokenName, () => this.TrinnieVariant);
      TokenRegistry.AddEnumToken<OnlyModdedVariant>(LennyTokenName, () => this.LennyVariant);
    } else {
      RegisterDummyTokens(BertTokenName, TrinnieTokenName, LennyTokenName);
    }
  }
}
