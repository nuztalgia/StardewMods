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
      HighlightWidth = BackgroundWidth - (PixelZoom * 2);
      SelectionBounds.Width = BackgroundWidth + ScaledButtonWidth;
      return (SelectionBounds.Width, DefaultHeight);
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

      public bool IsHovering { get; set; }
      public Action ClickAction { get; }

      private Rectangle HighlightBounds;

      private int ItemCount;
      private int Height;

      private int HoverIndex;
      private bool IsHoveringOnItem;

      internal Background(Action clickAction) {
        this.ClickAction = clickAction;
        this.HighlightBounds.Height = HighlightHeight;
      }

      internal int? GetHoverStatus() {
        return this.IsHoveringOnItem ? this.HoverIndex : null;
      }

      internal void UpdateItemCount(int itemCount) {
        this.ItemCount = itemCount;
        this.Height = (itemCount * ScaledHeight) + PixelZoom;
      }

      protected override (int width, int height) UpdateDimensions(int totalWidth) {
        this.HighlightBounds.Width = HighlightWidth;
        return (BackgroundWidth, this.Height);
      }

      protected override void Draw(SpriteBatch sb, Vector2 position) {
        // We only care about the selection bounds while the expansion is open (i.e. being drawn).
        SelectionBounds.X = (int) position.X;
        SelectionBounds.Y = (int) position.Y - ScaledHeight;

        this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, this.Height);

        this.HoverIndex = ((int) (MousePositionY - position.Y)) / ScaledHeight;
        this.IsHoveringOnItem = // Necessary check because HoverIndex is not guaranteed to be valid.
            this.IsHovering && (0 <= this.HoverIndex) && (this.HoverIndex < this.ItemCount);

        if (this.IsHoveringOnItem) {
          this.HighlightBounds.X = (int) position.X + PixelZoom;
          this.HighlightBounds.Y = (int) position.Y + (this.HoverIndex * ScaledHeight) + PixelZoom;
          DrawRectangle(sb, this.HighlightBounds, Color.Wheat);
        }
      }
    }

    private const int MaxDisplayedItemCount = 8;
    private const int ScrollDeltaPerItem = 120;

    private static readonly int BottomCutoff = Game1.uiViewport.Height - 116;

    private readonly int TotalItemCount;
    private readonly Action<int> ClickAction;
    private readonly Background BackgroundWidget;

    private int MaxScrollIndex => this.TotalItemCount - this.DisplayedItemCount;
    private int OutOfViewIndex => this.ScrollIndex + this.DisplayedItemCount;

    private bool IsExpanded;
    private bool IsScrollable;
    private int ScrollIndex;
    private int DisplayedItemCount;

    internal Expansion(IEnumerable<string> values, Action<int> clickAction)
        : this(values, clickAction, hideableEntries: new Dictionary<Widget, Func<bool>>()) {}

    private Expansion(
        IEnumerable<string> values,
        Action<int> clickAction,
        IDictionary<Widget, Func<bool>> hideableEntries)
            : base(hideableWidgets: hideableEntries) {

      this.TotalItemCount = values.Count();
      this.ClickAction = clickAction;
      this.BackgroundWidget = new Background(clickAction: () => this.TryConsumeClick());

      this.AddSubWidget(
          this.BackgroundWidget, preDraw: this.OnBackgroundDraw, postDraw: TextOffsetAdjustment);

      for (int i = 0; i < this.TotalItemCount; ++i) {
        int index = i; // Closure variable.
        Widget dropdownEntry = StaticText.CreateDropdownEntry(values.ElementAt(index));

        hideableEntries.Add(dropdownEntry, () => ShouldHideWidgetAtIndex(index));
        this.AddSubWidget(dropdownEntry,
            postDraw: (ref Vector2 position, int _, int _) => position.Y += ScaledHeight);
      }

      bool ShouldHideWidgetAtIndex(int index) {
        return this.IsScrollable && ((index < this.ScrollIndex) || (index >= this.OutOfViewIndex));
      }
    }

    public bool TryConsumeClick() {
      if (this.BackgroundWidget.GetHoverStatus() is int hoverIndex) {
        // The dropdown will handle the click, and any widgets underneath the clicked item will
        // be prevented from receiving/handling the click.
        this.ClickAction(hoverIndex + this.ScrollIndex);
        return true;
      } else {
        // Clicking anywhere outside of the expansion bounds will close the dropdown.
        this.ToggleExpandedState(forceValue: false);

        // If the click was inside the selection bounds, prevent the selection from receiving the
        // click (because the expected behavior is to close the dropdown, which was already done).
        return SelectionBounds.Contains(MousePositionX, MousePositionY);
      }
    }

    public void OnScrolled(int scrollDelta) {
      if (this.IsScrollable && this.BackgroundWidget.IsHovering) {
        this.ScrollIndex = Math.Clamp(
            this.ScrollIndex - (scrollDelta / ScrollDeltaPerItem), 0, this.MaxScrollIndex);
      }
    }

    public void OnDismissed() {
      this.IsExpanded = false;
      this.ScrollIndex = 0;
    }

    internal void ToggleExpandedState(bool? forceValue = null) {
      this.IsExpanded = forceValue ?? !this.IsExpanded;
      this.ScrollIndex = 0;
      this.SetOverlayStatus(isActive: this.IsExpanded);
    }

    private void OnBackgroundDraw(ref Vector2 position, int width, int height) {
      position.Y += ScaledHeight - PixelZoom;
      int displayedItemCount =
          Math.Min(MaxDisplayedItemCount, (BottomCutoff - (int) position.Y) / ScaledHeight);

      if (displayedItemCount != this.DisplayedItemCount) {
        this.DisplayedItemCount = displayedItemCount;
        this.IsScrollable = displayedItemCount < this.TotalItemCount;
        this.BackgroundWidget.UpdateItemCount(Math.Min(displayedItemCount, this.TotalItemCount));
      }
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
  private static int HighlightWidth;
  private static Rectangle SelectionBounds;

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
    SelectionBounds.Height = ScaledHeight;

    Selection selection = new(
        loadValue, saveValue, onValueChanged,
        clickAction: () => this.OnSelectionClicked?.Invoke());

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
