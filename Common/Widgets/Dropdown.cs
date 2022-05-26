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

    internal void SetValue(string value) {
      this.Value = value;
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

    private class Background : Widget, IHoverable, IClickable {

      private const int HighlightHeight = ScaledHeight - PixelZoom;

      private static int HighlightWidth;

      public Action ClickAction { get; }
      public bool IsHovering { get; set; }

      private readonly int ItemCount;
      private readonly int Height;

      private int HoverIndex;
      private bool IsHoveringOnItem;

      private Rectangle HighlightBounds;
      private Rectangle InteractionBounds;

      internal Background(Action clickAction, int itemCount) {
        this.ClickAction = clickAction;
        this.ItemCount = itemCount;
        this.Height = (itemCount * ScaledHeight) + PixelZoom;

        this.HighlightBounds.Height = HighlightHeight;
        this.InteractionBounds.Height = this.Height + ScaledHeight;
      }

      internal int? GetHoverInfo() {
        return this.IsHoveringOnItem ? this.HoverIndex : null;
      }

      internal bool HandlesCurrentMousePosition() {
        return this.InteractionBounds.Contains(MousePositionX, MousePositionY);
      }

      protected override (int width, int height) UpdateDimensions(int totalWidth) {
        HighlightWidth = BackgroundWidth - (PixelZoom * 2);
        this.HighlightBounds.Width = HighlightWidth;
        this.InteractionBounds.Width = BackgroundWidth;
        return (BackgroundWidth, this.Height);
      }

      protected override void Draw(SpriteBatch sb, Vector2 position) {
        this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, this.Height);

        this.HoverIndex = ((int) (MousePositionY - position.Y)) / ScaledHeight;
        this.IsHoveringOnItem =
            this.IsHovering && (0 <= this.HoverIndex) && (this.HoverIndex < this.ItemCount);

        this.InteractionBounds.X = (int) position.X;
        this.InteractionBounds.Y = (int) position.Y - ScaledHeight;

        if (this.IsHoveringOnItem) {
          this.HighlightBounds.X = (int) position.X + PixelZoom;
          this.HighlightBounds.Y = (int) position.Y + (this.HoverIndex * ScaledHeight) + PixelZoom;
          DrawRectangle(sb, this.HighlightBounds, Color.Wheat);
        }
      }
    }

    private readonly Background BackgroundWidget;
    private readonly Action<int> ClickAction;

    private bool IsExpanded = false;

    internal Expansion(IEnumerable<string> values, Action<int> clickAction) {
      this.ClickAction = clickAction;
      this.BackgroundWidget = new Background(() => this.TryConsumeClick(), values.Count());

      this.AddSubWidget(
          this.BackgroundWidget,
          preDraw: (ref Vector2 position, int _, int _) => position.Y += ScaledHeight - PixelZoom,
          postDraw: TextOffsetAdjustment);

      foreach (string value in values) {
        this.AddSubWidget(
            StaticText.CreateDropdownEntry(value),
            postDraw: (ref Vector2 position, int _, int _) => position.Y += ScaledHeight);
      }
    }

    public bool TryConsumeClick() {
      if (this.BackgroundWidget.GetHoverInfo() is int hoverIndex) {
        this.ClickAction(hoverIndex);
        return true;
      } else {
        this.ToggleExpandedState(forceValue: false);
        return this.BackgroundWidget.HandlesCurrentMousePosition();
      }
    }

    public void OnDismissed() {
      this.IsExpanded = false;
    }

    internal void ToggleExpandedState(bool? forceValue = null) {
      this.IsExpanded = forceValue ?? !this.IsExpanded;
      this.SetOverlayStatus(isActive: this.IsExpanded);
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
  private static readonly Adjustment TextOffsetAdjustment =
      (ref Vector2 position, int _, int _) => position += TextOffset;

  private static int BackgroundWidth;

  private readonly Action OnSelectionClicked;
  private readonly Action<int> OnExpansionClicked;

  internal Dropdown(
      string name,
      IEnumerable<string> allowedValues,
      Func<string> loadValue,
      Action<string> saveValue,
      Action<string>? onValueChanged = null,
      Func<string, string>? formatValue = null,
      string? tooltip = null) : base(name, tooltip) {

    string[] values = GetUniqueValues(allowedValues).ToArray();

    Selection selection = new(
        loadValue, saveValue, onValueChanged, () => this.OnSelectionClicked?.Invoke());

    Expansion expansion = new(
        values: (formatValue == null) ? values : values.Select(value => formatValue(value)),
        clickAction: (value) => this.OnExpansionClicked?.Invoke(value));

    TextWidget selectionText = DynamicText.CreateDropdownEntry(
        GetValueTextFunction(values, selection.GetValue, formatValue));

    this.OnSelectionClicked = () => expansion.ToggleExpandedState();
    this.OnExpansionClicked = (index) => {
      selection.SetValue(values[index]);
      expansion.ToggleExpandedState(forceValue: false);
    };

    this.AddSubWidget(selection);
    this.AddSubWidget(expansion);
    this.AddSubWidget(selectionText, preDraw: TextOffsetAdjustment);
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
