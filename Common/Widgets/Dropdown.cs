namespace Nuztalgia.StardewMods.Common.UI;

internal class Dropdown : Widget.Composite {

  private class Selection : OptionWidget<string>, IClickable {

    public Action ClickAction { get; }

    internal Selection(
        Func<string> loadValue,
        Action<string> saveValue,
        Action<string>? onValueChanged,
        Action clickAction)
            : base(loadValue, saveValue, onValueChanged) {
      this.ClickAction = clickAction;
    }

    internal string GetValue() {
      return this.Value;
    }

    protected override (int width, int height) UpdateDimensions(int totalWidth) {
      BackgroundWidth = (totalWidth / 2) - (ScaledButtonWidth * 2) - BackgroundPadding;
      return (BackgroundWidth + ScaledButtonWidth, DefaultHeight);
    }

    protected override void Draw(SpriteBatch sb, Vector2 position) {
      this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, ScaledHeight);
      position.X += BackgroundWidth - PixelZoom;
      DrawFromCursors(sb, position, ButtonSourceRect);
    }
  }

  private class Expansion : Composite, IOverlayable {

    private class Background : Widget {

      private readonly int Height;

      internal Background(int height) {
        this.Height = height;
      }

      protected override (int width, int height) UpdateDimensions(int totalWidth) {
        return (BackgroundWidth, this.Height);
      }

      protected override void Draw(SpriteBatch sb, Vector2 position) {
        this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, this.Height);
      }
    }

    private bool IsExpanded = false;

    internal Expansion(IEnumerable<string> values) {
      this.AddSubWidget(
          new Background((ScaledHeight * values.Count()) + PixelZoom),
          postDraw: MainTextPreDrawAdjustment);

      foreach (string value in values) {
        this.AddSubWidget(
            StaticText.CreateDropdownEntry(value),
            postDraw: ExpandedTextPostDrawAdjustment);
      }
    }

    internal void ToggleExpandedState() {
      this.IsExpanded = !this.IsExpanded;
      (this as IOverlayable).SetOverlayStatus(isActive: this.IsExpanded);
    }
  }

  private const int BackgroundPadding = 64;
  private const int RawButtonWidth = 10;
  private const int RawHeight = 11;

  private const int ScaledButtonWidth = RawButtonWidth * PixelZoom;
  private const int ScaledHeight = RawHeight * PixelZoom;

  private static readonly Rectangle ButtonSourceRect = new(437, 450, RawButtonWidth, RawHeight);
  private static readonly Rectangle BackgroundSourceRect = new(433, 451, 3, 3);

  private static readonly Vector2 TextOffset = new(x: 12, y: 9);
  private static readonly Adjustment MainTextPreDrawAdjustment =
      (ref Vector2 position, int width, int height) => position += TextOffset;
  private static readonly Adjustment ExpandedTextPostDrawAdjustment =
      (ref Vector2 position, int width, int height) => position.Y += ScaledHeight;

  private static int BackgroundWidth;

  internal Dropdown(
      string name,
      IEnumerable<string> allowedValues,
      Func<string> loadValue,
      Action<string> saveValue,
      Action<string>? onValueChanged = null,
      Func<string, string>? formatValue = null,
      string? tooltip = null) : base(name, tooltip) {

    IEnumerable<string> values = GetUniqueValues(allowedValues);
    Expansion expansion = new(
        (formatValue == null) ? values : values.Select(value => formatValue(value)));

    Selection selection = new(loadValue, saveValue, onValueChanged, expansion.ToggleExpandedState);
    Func<string> getValueText = GetValueTextFunction(values, selection.GetValue, formatValue);

    this.AddSubWidget(expansion);
    this.AddSubWidget(selection);
    this.AddSubWidget(DynamicText.CreateDropdownEntry(getValueText), MainTextPreDrawAdjustment);
  }

  private static IEnumerable<string> GetUniqueValues(IEnumerable<string> allValues) {
    HashSet<string> returnedValues = new();
    foreach (string value in allValues) {
      if (!returnedValues.Contains(value)) {
        yield return value;
        returnedValues.Add(value);
      }
    }
  }

  private static Func<string> GetValueTextFunction(
      IEnumerable<string> uniqueValues, Func<string> getValue, Func<string, string>? formatValue) {
    if (formatValue == null) {
      return getValue;
    } else {
      Dictionary<string, string> formattedValues = new();
      uniqueValues.ForEach(value => formattedValues.Add(value, formatValue(value)));
      return () => formattedValues[getValue()];
    }
  }
}
