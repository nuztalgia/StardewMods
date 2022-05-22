namespace Nuztalgia.StardewMods.Common;

internal interface IConfigPageBuilder {

  IConfigMenuBuilder PublishPage();

  IConfigPageBuilder AddStaticHeader(string text);

  IConfigPageBuilder AddStaticHeader(Func<string> getText);

  IConfigPageBuilder AddStaticText(string text);

  IConfigPageBuilder AddStaticText(Func<string> getText);

  IConfigPageBuilder AddVerticalSpacing();

  IConfigPageBuilder AddVerticalSpacing(int height);

  IConfigPageBuilder AddStaticHeaderWithButton(
      string headerText,
      string buttonText,
      Action buttonAction);

  IConfigPageBuilder AddCheckboxOption(
      string name,
      Func<bool> loadValue,
      Action<bool> saveValue,
      Action<bool>? onValueChanged = null,
      string? tooltip = null,
      Func<bool>? hideWhen = null);

  IConfigPageBuilder AddSliderOption(
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
}
