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

      public Action ClickAction { get; }

      public bool IsHovering {
        get => this.IsHovering_Field && this.IsIndexInBounds(this.HoverIndex);
        set {
          if (value != this.IsHovering_Field) {
            this.IsHovering_Field = value;
            TryPlaySound(this.IsHovering ? HoverSoundName : null);
          }
        }
      }

      internal int HoverIndex {
        get => this.HoverIndex_Field;
        private set {
          value = Math.Clamp(value, SelectionWidgetIndex - 1, this.ItemCount);
          if (value != this.HoverIndex_Field) {
            this.HoverIndex_Field = value;
            TryPlaySound(this.IsHovering ? HoverSoundName : null);
          }
        }
      }

      internal int ItemCount;
      internal int Height;

      private readonly Func<int> GetRelativeSelectedIndex;

      private Rectangle HighlightBounds = new(0, 0, 0, height: ScaledHeight - PixelZoom);

      private bool IsHovering_Field = false;
      private int HoverIndex_Field = SelectionWidgetIndex;

      internal Background(Action clickAction, Func<int> getRelativeSelectedIndex) {
        this.ClickAction = clickAction;
        this.GetRelativeSelectedIndex = getRelativeSelectedIndex;
      }

      protected override (int width, int height) UpdateDimensions(int totalWidth) {
        this.HighlightBounds.Width = HighlightWidth;
        return (BackgroundWidth, this.Height);
      }

      protected override void Draw(SpriteBatch sb, Vector2 position) {
        this.HoverIndex = (int) Math.Floor((MousePositionY - position.Y) / ScaledHeight);
        this.DrawFromCursors(sb, position, BackgroundSourceRect, BackgroundWidth, this.Height);

        int drawIndex = this.IsHovering ? this.HoverIndex : this.GetRelativeSelectedIndex();

        if (this.IsIndexInBounds(drawIndex)) {
          this.HighlightBounds.X = (int) position.X + PixelZoom;
          this.HighlightBounds.Y = (int) position.Y + (drawIndex * ScaledHeight) + PixelZoom;
          DrawRectangle(sb, this.HighlightBounds, Color.Wheat);
        }
      }

      private bool IsIndexInBounds(int index) {
        return (0 <= index) && (index < this.ItemCount);
      }
    }

    private class Scrollbar : Widget, IDraggable {

      private const int RawWidth = 6;
      private const int RawBarHeight = 10;
      private const int RawTrackHeight = 6;

      private static readonly Rectangle BarSourceRect = new(435, 463, RawWidth, RawBarHeight);
      private static readonly Rectangle TrackSourceRect = new(403, 383, RawWidth, RawTrackHeight);

      private readonly Func<int> GetMaxScrollIndex;
      private readonly Action<int> SetScrollIndex;

      public bool IsDragging { get; set; }

      private bool IsScrollable;
      private int TrackHeight;
      private int HeightMultiplier;
      private int VerticalBarOffset;

      internal Scrollbar(Func<int> getMaxScrollIndex, Action<int> setScrollIndex) {
        this.GetMaxScrollIndex = getMaxScrollIndex;
        this.SetScrollIndex = setScrollIndex;
      }

      internal void UpdateDrawInfo(bool isScrollable, int trackHeight) {
        this.IsScrollable = isScrollable;
        this.TrackHeight = trackHeight;
        this.HeightMultiplier = trackHeight - (RawBarHeight * PixelZoom);
      }

      internal void UpdateScrollIndex(int scrollIndex) {
        float percentScrolled = (float) scrollIndex / this.GetMaxScrollIndex();
        this.VerticalBarOffset = (int) (percentScrolled * this.HeightMultiplier);
      }

      protected override (int width, int height) UpdateDimensions(int totalWidth) {
        return this.IsScrollable ? (RawWidth * PixelZoom, this.TrackHeight) : (0, 0);
      }

      protected override void Draw(SpriteBatch sb, Vector2 position) {
        if (!this.IsScrollable) {
          return;
        }

        if (this.IsDragging) {
          int maxScrollIndex = this.GetMaxScrollIndex();
          float unboundedIndex = (MousePositionY - position.Y) / this.TrackHeight * maxScrollIndex;
          this.SetScrollIndex(Math.Clamp((int) unboundedIndex, 0, maxScrollIndex));
        }

        this.DrawFromCursors(sb, position, TrackSourceRect);
        position.Y += this.VerticalBarOffset;
        DrawFromCursors(sb, position, BarSourceRect);
      }
    }

    private const string ToggleSoundName = "drumkit6";
    private const string HoverSoundName = "Cowboy_Footstep";
    private const string ScrollSoundName = "shwip";

    private const int SelectionWidgetIndex = -1;
    private const int MaxDisplayedItemCount = 6;
    private const int ScrollDeltaPerItem = 120;

    private static readonly int BottomCutoff = Game1.uiViewport.Height - 128;

    private readonly int TotalItemCount;
    private readonly Action<int> ClickAction;
    private readonly Func<int> GetSelectedIndex;

    private readonly Background BackgroundWidget;
    private readonly Scrollbar ScrollbarWidget;

    private int MaxScrollIndex => this.TotalItemCount - this.DisplayedItemCount;
    private int OutOfViewIndex => this.ScrollIndex + this.DisplayedItemCount;

    private int ScrollIndex {
      get => this.ScrollIndex_Field;
      set {
        value = Math.Clamp(value, 0, this.MaxScrollIndex);

        if (value != this.ScrollIndex_Field) {
          // Don't play this sound on the very first expansion, when there hasn't been a scroll yet.
          TryPlaySound((this.ShouldPlayScrollSound && this.IsExpanded) ? ScrollSoundName : null);
          this.ShouldPlayScrollSound = true;

          this.ScrollIndex_Field = value;
          this.ScrollbarWidget.UpdateScrollIndex(this.ScrollIndex_Field);
        }
      }
    }

    private bool IsExpanded {
      get => this.IsExpanded_Field;
      set {
        if (value != this.IsExpanded_Field) {
          int selectedIndex = this.GetSelectedIndex();
          bool playToggleSound = value || (selectedIndex != this.ReferenceIndex);
          TryPlaySound(playToggleSound ? ToggleSoundName : HoverSoundName);

          this.ReferenceIndex = selectedIndex;
          this.IsExpanded_Field = value;
        }
      }
    }

    private int ReferenceIndex;
    private int DisplayedItemCount;
    private bool IsScrollable;
    private bool ShouldPlayScrollSound;

    private int ScrollIndex_Field;
    private bool IsExpanded_Field;

    internal Expansion(
        IEnumerable<string> values, Action<int> clickAction, Func<int> getSelectedIndex)
            : this(values, clickAction, getSelectedIndex, new Dictionary<Widget, Func<bool>>()) { }

    private Expansion(
        IEnumerable<string> values,
        Action<int> clickAction,
        Func<int> getSelectedIndex,
        IDictionary<Widget, Func<bool>> hideableEntries)
            : base(hideableWidgets: hideableEntries) {

      this.TotalItemCount = values.Count();
      this.ClickAction = clickAction;
      this.GetSelectedIndex = getSelectedIndex;

      this.ReferenceIndex = getSelectedIndex();

      this.BackgroundWidget = new Background(
          clickAction: () => this.TryConsumeClick(),
          getRelativeSelectedIndex: () => this.GetSelectedIndex() - this.ScrollIndex);

      this.ScrollbarWidget = new Scrollbar(
          getMaxScrollIndex: () => this.MaxScrollIndex,
          setScrollIndex: (index) => this.ScrollIndex = index);

      this.AddSubWidget(this.BackgroundWidget, preDraw: this.OnBackgroundDraw);
      this.AddSubWidget(this.ScrollbarWidget,
          preDraw: this.BeforeScrollbarDraw, postDraw: this.AfterScrollbarDraw);

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
      if (this.ScrollbarWidget.IsDragging) {
        // Prevent the background and other widgets from receiving the end-of-scrollbar-drag click.
        return true;
      } else if (this.BackgroundWidget.IsHovering) {
        // The dropdown's background (i.e. this expansion) will handle the click, and any widgets
        // underneath the clicked item will be prevented from receiving/handling the click.
        this.ClickAction(this.BackgroundWidget.HoverIndex + this.ScrollIndex);
        return true;
      } else {
        // Clicking anywhere outside of the expansion bounds will close the dropdown.
        this.ToggleExpandedState(forceValue: false);

        // If the click was inside the selection bounds, prevent the selection from receiving the
        // click (because the expected behavior is to close the dropdown, which was already done).
        return this.BackgroundWidget.HoverIndex == SelectionWidgetIndex;
      }
    }

    public void OnScrolled(int scrollDelta) {
      if (this.IsScrollable && this.BackgroundWidget.IsHovering) {
        this.ScrollIndex -= scrollDelta / ScrollDeltaPerItem;
      }
    }

    public void OnDismissed() {
      this.IsExpanded = false;
      this.ResetScrollIndex();
    }

    internal void ToggleExpandedState(bool? forceValue = null) {
      this.IsExpanded = forceValue ?? !this.IsExpanded;
      this.ResetScrollIndex();
      this.SetOverlayStatus(isActive: this.IsExpanded);
    }

    private void ResetScrollIndex() {
      if (this.DisplayedItemCount > 0) {
        this.ScrollIndex = this.GetSelectedIndex() - (this.DisplayedItemCount / 2) + 1;
      }
    }

    private void OnBackgroundDraw(ref Vector2 position, int width, int height) {
      position.Y += ScaledHeight - PixelZoom;
      int displayedItemCount =
          Math.Min(MaxDisplayedItemCount, (BottomCutoff - (int) position.Y) / ScaledHeight);

      if (displayedItemCount != this.DisplayedItemCount) {
        this.IsScrollable = displayedItemCount < this.TotalItemCount;
        this.DisplayedItemCount = this.IsScrollable ? displayedItemCount : this.TotalItemCount;
        this.ResetScrollIndex();

        this.BackgroundWidget.ItemCount = Math.Min(displayedItemCount, this.TotalItemCount);
        this.BackgroundWidget.Height = (this.BackgroundWidget.ItemCount * ScaledHeight) + PixelZoom;

        this.ScrollbarWidget.UpdateDrawInfo(this.IsScrollable, this.BackgroundWidget.Height);
      }
    }

    private void BeforeScrollbarDraw(ref Vector2 position, int width, int height) {
      if (this.IsScrollable) {
        position.X += BackgroundWidth - width;
      }
    }

    private void AfterScrollbarDraw(ref Vector2 position, int width, int height) {
      if (this.IsScrollable) {
        position.X -= BackgroundWidth - width;
      }
      position += TextOffset;
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

  private static int BackgroundWidth;
  private static int HighlightWidth;

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
        loadValue, saveValue, onValueChanged,
        clickAction: () => this.OnSelectionClicked?.Invoke());

    Expansion expansion = new(
        values: (formatValue == null) ? values : values.Select(value => formatValue(value)),
        clickAction: (value) => this.OnExpansionClicked?.Invoke(value),
        getSelectedIndex: () => Array.IndexOf(values, selection.GetValue()));

    TextWidget selectionText = DynamicText.CreateDropdownEntry(
        GetValueTextFunction(values, selection.GetValue, formatValue));

    this.OnSelectionClicked = () => expansion.ToggleExpandedState();
    this.OnExpansionClicked = (index) => {
      selection.SetValue(values[index]);
      expansion.ToggleExpandedState(forceValue: false);
    };

    this.AddSubWidget(selection);
    this.AddSubWidget(expansion);
    this.AddSubWidget(selectionText,
        preDraw: (ref Vector2 position, int _, int _) => position += TextOffset);
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
