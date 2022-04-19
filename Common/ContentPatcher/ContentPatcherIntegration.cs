using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ContentPatcher;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common;

internal sealed class ContentPatcherIntegration : BaseIntegration<IContentPatcherAPI> {

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private ContentPatcherIntegration(IContentPatcherAPI api, IManifest manifest)
      : base(api, manifest) { }

  internal void RegisterToken(string tokenName, Func<IEnumerable<string>> getTokenValue) {
    string currentValue = string.Join(", ", getTokenValue());
    Log.Verbose($"Registering Token: {tokenName,-28}|  Current Value: '{currentValue}'");
    this.Api.RegisterToken(this.Manifest, tokenName, getTokenValue);
  }
}
