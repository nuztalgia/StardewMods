using System.Collections.Generic;
using Nuztalgia.StardewMods.Common.ContentPatcher;

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

  protected override void RegisterAllTokens(Integration contentPatcher) {
    contentPatcher.RegisterEnumToken(BertTokenName, () => this.BertVariant);
    contentPatcher.RegisterEnumToken(TrinnieTokenName, () => this.TrinnieVariant);
    contentPatcher.RegisterEnumToken(LennyTokenName, () => this.LennyVariant);
  }

  protected override IEnumerable<string> GetTokenNames() {
    yield return BertTokenName;
    yield return TrinnieTokenName;
    yield return LennyTokenName;
  }
}
