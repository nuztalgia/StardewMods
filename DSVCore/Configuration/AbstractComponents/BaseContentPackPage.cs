using System;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseContentPackPage : BaseMenuPage {

  private const string RootModId = "DSVTeam.DiverseSeasonalOutfits";

  private readonly string ContentPackId;
  private readonly Lazy<IModContentHelper> ModContentHelper;

  internal BaseContentPackPage() {
    this.ContentPackId = $"{RootModId}.{this.Name}";
    this.ModContentHelper = new Lazy<IModContentHelper>(() => {
      // "This is really bad. Pathos don't kill me."  - kittycatcasey
      IModInfo modInfo = Globals.ModRegistry.Get(this.ContentPackId);
      object? questionableObject = modInfo.GetType().GetProperty("ContentPack")?.GetValue(modInfo);
      return ((IContentPack) questionableObject!).ModContent;
    });
  }

  // This method will produce a big red error if called on a content pack that isn't installed.
  // We purposely don't catch the error so that it's extremely obvious if we dun goofed. :^)
  internal IModContentHelper GetModContentHelper() {
    return this.ModContentHelper.Value;
  }

  internal override string GetDisplayName() {
    return Globals.GetI18nString($"Page_{this.Name}") ?? this.Name;
  }

  internal override bool IsAvailable() {
    return Globals.ModRegistry.IsLoaded(this.ContentPackId);
  }
}
