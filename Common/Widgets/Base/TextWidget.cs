namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class TextWidget : Widget {

  protected delegate int MeasureWidth(string text);

  protected abstract string RawText { get; }
  protected abstract int LineHeight { get; }
  protected abstract MeasureWidth MeasureTextWidth { get; }

  private readonly bool WrapLines;

  private string[]? SplitLines;

  protected TextWidget(bool wrapLines, Alignment? alignment) : base(alignment) {
    this.WrapLines = wrapLines;
  }

  protected abstract void Draw(SpriteBatch sb, Vector2 position, string text);

  protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
    if (this.SplitLines == null) {
      this.Draw(sb, position, this.RawText);
    } else {
      foreach (string line in this.SplitLines) {
        this.Draw(sb, position, line);
        position.Y += this.LineHeight;
      }
    }
  }

  protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
    int singleLineWidth = this.MeasureTextWidth(this.RawText);
    if (this.WrapLines && (singleLineWidth > totalWidth)) {
      this.SplitLines = this.GetSplitLines(totalWidth).ToArray();
      return (totalWidth, this.SplitLines.Length * this.LineHeight);
    } else {
      return (Math.Min(singleLineWidth, totalWidth), this.LineHeight);
    }
  }

  private IEnumerable<string> GetSplitLines(int maxLineWidth) {
    string currentLine = "";

    foreach (string word in this.RawText.Split(' ')) {
      string possibleLine = $"{currentLine} {word}".Trim();

      if (this.MeasureTextWidth(possibleLine) > maxLineWidth) {
        yield return currentLine;
        currentLine = word;
      } else {
        currentLine = possibleLine;
      }
    }

    if (!currentLine.IsEmpty()) {
      yield return currentLine;
    }
  }
}
