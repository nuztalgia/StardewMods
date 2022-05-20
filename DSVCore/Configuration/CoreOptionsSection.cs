namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class CoreOptionsSection : BaseMenuSection {

  internal enum PyjamaHabits {
    True,
    False,
    Marriage
  }

  public PyjamaHabits Pyjamas { get; set; } = PyjamaHabits.True;
  public bool MermaidPendants { get; set; } = true;
  public bool MaternitySprites { get; set; } = false;

  internal override void RegisterTokens(ContentPatcherIntegration contentPatcher) {
    // TODO: Shift more of the computation for the Pyjamas token from the content packs to this mod.
    contentPatcher.RegisterEnumToken("Pyjamas", () => this.Pyjamas);
    contentPatcher.RegisterBoolToken("MermaidPendants", () => this.MermaidPendants);
    contentPatcher.RegisterBoolToken("MaternitySprites", () => this.MaternitySprites);
  }

  internal override string GetDisplayName() {
    return I18n.Core_Section_Title();
  }

  internal override bool IsAvailable() {
    // This section is part of the core mod (a.k.a. this mod), so it's always available.
    return true;
  }

  protected override string? GetTooltip(PropertyInfo property) {
    return I18nHelper.GetStringByKeyName($"Tooltip_{property.Name}");
  }
}
