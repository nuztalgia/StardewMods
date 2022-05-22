namespace Nuztalgia.StardewMods.Common.UI;

internal class Dropdown : Widget.Composite {

  private class Selection : OptionWidget<string>, IClickable {

    private const int RawButtonWidth = 10;
    private const int RawHeight = 11;

    private const int ScaledButtonWidth = RawButtonWidth * PixelZoom;
    private const int ScaledHeight = RawHeight * PixelZoom;

    private static readonly Rectangle BackgroundSourceRect = new(433, 451, 3, 3);
    private static readonly Rectangle ButtonSourceRect = new(437, 450, RawButtonWidth, RawHeight);

    private static int BackgroundWidth;

    public Action ClickAction => () => Log.Error("Dropdowns aren't properly implemented yet!");

    internal Selection(
        Func<string> loadValue,
        Action<string> saveValue,
        Action<string>? onValueChanged)
            : base(loadValue, saveValue, onValueChanged) { }

    internal string GetValue() {
      return this.Value;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      BackgroundWidth = (totalWidth / 2) - (ScaledButtonWidth * 2) - 64;
      return (BackgroundWidth + ScaledButtonWidth, DefaultHeight);
    }

    protected override void Draw(SpriteBatch sb, Vector2 position) {
      this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, ScaledHeight);
      position.X += BackgroundWidth - PixelZoom;
      DrawFromCursors(sb, position, ButtonSourceRect);
    }
  }

  private static readonly Vector2 TextAdjustment = new(x: 10, y: 1);

  internal Dropdown(
      string name,
      IEnumerable<string> allowedValues,
      Func<string> loadValue,
      Action<string> saveValue,
      Action<string>? onValueChanged = null,
      Func<string, string>? formatValue = null,
      string? tooltip = null) : base(name, tooltip) {

    List<string> uniqueValues = new();
    foreach (string value in allowedValues) {
      if (!uniqueValues.Contains(value)) {
        uniqueValues.Add(value);
      }
    }

    Selection selection = new(loadValue, saveValue, onValueChanged);
    Func<string> getValueText = () => selection.GetValue();

    if (formatValue != null) {
      Dictionary<string, string> formattedValues = new();
      uniqueValues.ForEach(value => formattedValues.Add(value, formatValue(value)));
      getValueText = () => formattedValues[selection.GetValue()];
    }

    this.AddSubWidget(selection);
    this.AddSubWidget(DynamicText.CreateDropdownEntry(getValueText),
        preDraw: (ref Vector2 position, int _, int _) => position += TextAdjustment);
  }
}
