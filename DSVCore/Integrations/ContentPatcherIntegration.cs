using System;
using System.Collections.Generic;
using System.Reflection;
using ContentPatcher;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal class ContentPatcherIntegration : BaseIntegration<IContentPatcherAPI> {

  internal ContentPatcherIntegration() : base(Globals.ModRegistry, "Pathoschild.ContentPatcher") { }

  // Returns true if all tokens were registered successfully.
  internal bool RegisterTokens() {
    if (this.Api is null) {
      Log.Error("Could not retrieve the Content Patcher API. This mod will not function at all.");
      return false;
    }

    Dictionary<string, Func<IEnumerable<string>>> tokenMap = new();

    foreach (PropertyInfo property in Globals.Config.GetType().GetProperties()) {
      if (property.GetValue(Globals.Config) is BaseMenuPage page) {
        page.AddTokens(tokenMap);
      }
    }

    foreach (KeyValuePair<string, Func<IEnumerable<string>>> token in tokenMap) {
      string tokenValue = string.Join(", ", token.Value!());
      Log.Verbose($"Token Name: {token.Key,-28}|  Current Value: {tokenValue}");
      this.Api.RegisterToken(Globals.Manifest, token.Key, token.Value);
    }

    return true;
  }
}
