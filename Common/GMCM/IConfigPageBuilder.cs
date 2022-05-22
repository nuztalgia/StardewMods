namespace Nuztalgia.StardewMods.Common;

internal interface IConfigPageBuilder {

  IConfigPageBuilder AddSpacing();

  IConfigPageBuilder AddParagraph(string text);

  IConfigPageBuilder AddHeader(string text);

  IConfigPageBuilder AddHeaderWithButton(string headerText, string buttonText, Action buttonAction);

  IConfigPageBuilder AddCheckbox(
      string name,
      Func<bool> loadValue,
      Action<bool> saveValue,
      Action<bool>? onValueChanged = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null);

  IConfigPageBuilder AddSlider(
      string name,
      Func<int> loadValue,
      Action<int> saveValue,
      Action<int>? onValueChanged = null,
      int? staticMinValue = null,
      int? staticMaxValue = null,
      Func<int>? getDynamicMinValue = null,
      Func<int>? getDynamicMaxValue = null,
      Func<int, string>? formatValue = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null);

  IConfigMenuBuilder PublishPage();
}
