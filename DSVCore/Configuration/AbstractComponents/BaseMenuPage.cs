namespace Nuztalgia.StardewMods.DSVCore;

internal abstract class BaseMenuPage : BaseMenuComponent {

  internal IEnumerable<BaseMenuSection> GetAllSections() {
    foreach (PropertyInfo property in this.GetType().GetProperties()) {
      if (property.GetValue(this) is BaseMenuSection section) {
        yield return section;
      } else {
        Log.Error(
            $"Invalid type '{property.PropertyType}' for property " +
            $"'{property.Name}' on page '{this.GetDisplayName()}'.");
      }
    }
  }

  internal override void RegisterTokens(ContentPatcherIntegration contentPatcher) {
    foreach (BaseMenuSection section in this.GetAllSections()) {
      section.RegisterTokens(contentPatcher);
    }
  }
}
