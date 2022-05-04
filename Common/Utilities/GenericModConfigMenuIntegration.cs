using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common.GenericModConfigMenu;

internal sealed class Integration : BaseIntegration<IGenericModConfigMenuApi> {

  [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification =
      "This class is only instantiated in BaseMod, which uses reflection to get this constructor.")]
  private Integration(IGenericModConfigMenuApi api, IManifest manifest) : base(api, manifest) { }

  internal Integration Register(Action resetAction, Action saveAction) {
    this.Api.Register(this.Manifest, resetAction, saveAction);
    return this;
  }

  internal Integration OnFieldChanged(Action<string, object> onFieldChanged) {
    this.Api.OnFieldChanged(this.Manifest, onFieldChanged);
    return this;
  }

  internal Integration AddSpacing() {
    return this.AddParagraph("\n");
  }

  internal Integration AddSpacing(int size) {
    return this.AddParagraph(string.Join(" ", Enumerable.Repeat('\n', size)));
  }

  internal Integration AddSectionTitle(string text) {
    return this.AddSectionTitle(() => text);
  }

  internal Integration AddSectionTitle(Func<string> text) {
    this.Api.AddSectionTitle(this.Manifest, text);
    return this;
  }

  internal Integration AddParagraph(string text) {
    this.AddParagraph(() => text);
    return this;
  }

  internal Integration AddParagraph(Func<string> text) {
    this.Api.AddParagraph(this.Manifest, text);
    return this;
  }

  internal Integration AddPage(string pageId, string pageTitle) {
    this.Api.AddPage(this.Manifest, pageId, () => pageTitle);
    return this;
  }

  internal Integration AddPageLink(string pageId, string text) {
    this.Api.AddPageLink(this.Manifest, pageId, () => text);
    return this;
  }

  internal Integration AddEnumOption(
      object container,
      PropertyInfo property,
      string optionName,
      Func<string, string>? formatValue = null,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddTextOption(
        mod: this.Manifest,
        name: () => optionName,
        getValue: () => property.GetValue(container)?.ToString() ?? string.Empty,
        setValue: value => property.SetValue(container, Enum.Parse(property.PropertyType, value)),
        allowedValues: Enum.GetNames(property.PropertyType),
        formatAllowedValue: formatValue,
        tooltip: (tooltip is null) ? null : () => tooltip,
        fieldId: fieldId
    );

    return this;
  }

  internal Integration AddBoolOption(
      object container,
      PropertyInfo property,
      string optionName,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddBoolOption(
        mod: this.Manifest,
        name: () => optionName,
        getValue: () => ((bool?) property.GetValue(container)) ?? false,
        setValue: value => property.SetValue(container, value),
        tooltip: (tooltip is null) ? null : () => tooltip,
        fieldId: fieldId
    );

    return this;
  }

  internal Integration AddIntOption(
      object container,
      PropertyInfo property,
      string optionName, 
      int? min = null,
      int? max = null,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddNumberOption(
        mod: this.Manifest,
        name: () => optionName,
        getValue: () => ((int?) property.GetValue(container)) ?? 0,
        setValue: value => property.SetValue(container, value),
        min: min,
        max: max,
        tooltip: (tooltip is null) ? null : () => tooltip,
        fieldId: fieldId
    );

    return this;
  }

  internal Integration AddComplexOption(
      string optionName,
      Func<int> getHeight,
      Action<SpriteBatch, Vector2> drawAction,
      Action? resetAction = null,
      Action? saveAction = null,
      string? tooltip = null,
      string? fieldId = null) {

    this.Api.AddComplexOption(
        mod: this.Manifest,
        name: () => optionName,
        draw: drawAction,
        tooltip: (tooltip is null) ? null : () => tooltip,
        beforeMenuOpened: resetAction, // Make sure the option reflects the actual current state.
        beforeMenuClosed: resetAction, // Option state should be reset when the menu page is closed.
        beforeReset: resetAction,
        beforeSave: () => saveAction?.Invoke(), // GMCM doesn't support null actions for beforeSave.
        height: getHeight,
        fieldId: fieldId
    );

    return this;
  }
}
