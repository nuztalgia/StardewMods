using System;
using System.Collections.Generic;

namespace Nuztalgia.StardewMods.DSVCore.CompatSections;

internal abstract class RidgesideVillage : BaseCompatSection {

  private const string ModId = "Rafseazz.RidgesideVillage";
  private const string ModName = "Ridgeside Village";

  // Individual section on the "Abigail, Caroline & Pierre" content pack page.
  internal sealed class Bert : RidgesideVillage {
    public OnlyModdedVariant Variant { get; set; } = OnlyModdedVariant.Off;
  }

  // Individual section on the "Harvey, Gus & Lewis" content pack page.
  internal sealed class Lenny : RidgesideVillage {
    public OnlyModdedVariant Variant { get; set; } = OnlyModdedVariant.Off;
  }

  // Individual section on the "Abigail, Caroline & Pierre" content pack page.
  internal sealed class Trinnie : RidgesideVillage {
    public TrinnieVariant Variant { get; set; } = TrinnieVariant.Off;
  }

  internal enum OnlyModdedVariant {
    Modded, Off
  }

  internal enum TrinnieVariant {
    ModdedBlack,
    ModdedGreen,
    Off
  }

  internal RidgesideVillage() : base(ModId, ModName) { }

  internal override void AddTokens(Dictionary<string, Func<IEnumerable<string>>> tokenMap) {
    // TODO: Implement.
  }

  internal override string GetDisplayName() {
    return $"{ModName}: {this.Name}";
  }
}
