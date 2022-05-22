namespace Nuztalgia.StardewMods.Common.UI;

internal sealed class MenuPage : Widget.Composite {

  private const int VerticalSpacing = 16;

  internal MenuPage(
      IEnumerable<Widget> widgetsInOrder, IDictionary<Widget, Func<bool>>? hideableWidgets)
          : base(hideableWidgets, canDrawOverlay: true) {

    widgetsInOrder.ForEach((Widget widget) => this.AddSubWidget(widget,
        postDraw: (ref Vector2 position, int _, int _) => position.Y += VerticalSpacing));
  }
}
