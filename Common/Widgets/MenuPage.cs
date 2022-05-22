namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal sealed class MenuPage : Composite {

    internal MenuPage(
        IEnumerable<Widget> widgetsInOrder, IDictionary<Widget, Func<bool>?> widgetsHideWhen)
            : base(linearMode: LinearMode.Vertical, isFullWidth: true) {

      widgetsInOrder.ForEach((Widget widget) => this.AddSubWidget(widget));
      // TODO: Prevent subwidgets from being drawn if their "hideWhen" is true.
    }

    protected override void PostDraw(SpriteBatch sb, Vector2 position) {
      ActiveOverlay?.Draw(sb, position);
    }
  }
}
