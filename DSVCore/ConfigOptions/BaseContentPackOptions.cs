using Nuztalgia.StardewMods.Common;
using System;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackOptions : BaseOptions {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";

  private readonly string ContentPackId;
  private readonly Lazy<string> DisplayName;

  internal BaseContentPackOptions() {
    this.ContentPackId = $"{RootModId}.{this.Name}";
    Log.Verbose($"Ready to handle content pack: {this.ContentPackId}");

    this.DisplayName = new(() => Globals.GetI18nString($"Pack_{this.Name}_Name"));
  }

  internal string GetDisplayName() {
    return this.DisplayName.Value;
  }

  internal bool IsContentPackLoaded() {
    return Globals.ModRegistry.IsLoaded(this.ContentPackId);
  }
}
