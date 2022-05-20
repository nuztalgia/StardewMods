namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseSyncedCompatSection : BaseCompatSection {

  private readonly string TokenName;
  private readonly string ConfigKey;

  internal BaseSyncedCompatSection(string modId, string modName, string tokenName, string configKey)
      : base(modId, modName, new string[] { tokenName }) {
    this.TokenName = tokenName;
    this.ConfigKey = configKey;
  }

  internal IEnumerable<string>? GetSyncedItems() {
    return (this.GetConfigValue() is string configValue) ? configValue.CommaSplit() : null;
  }

  protected override sealed void RegisterCompatTokens(ContentPatcherIntegration contentPatcher) {
    contentPatcher.RegisterStringToken(this.TokenName, () => this.GetConfigValue() ?? string.Empty);
  }

  private string? GetConfigValue() {
    if ((ModRegistryUtils.LoadConfigFromContentPack(this.ModId) is Dictionary<string, string> dict)
        && dict.TryGetValue(this.ConfigKey, out string? configValue)) {
      Log.Trace($"Successfully retrieved config value '{configValue}' from mod '{this.ModName}'.");
      return configValue;
    }
    return null;
  }
}
