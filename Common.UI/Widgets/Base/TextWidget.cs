using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class TextWidget : Widget {

  protected abstract string RawText { get; }
  protected abstract int SingleLineWidth { get; }
  protected abstract int SingleLineHeight { get; }

  private readonly bool WrapLines;

  private string[]? SplitLines;

  protected TextWidget(Alignment? alignment, bool wrapLines) : base(alignment) {
    this.WrapLines = wrapLines;
  }

  internal abstract Vector2 MeasureSingleLine(string text);

  protected abstract void Draw(SpriteBatch sb, Vector2 position, string text);

  protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
    if (this.SplitLines == null) {
      this.Draw(sb, position, this.RawText);
    } else {
      foreach (string line in this.SplitLines) {
        this.Draw(sb, position, line);
        position.Y += this.SingleLineHeight;
      }
    }
  }

  protected override sealed (int width, int height) UpdateDimensions(int totalWidth) {
    if (this.WrapLines && (this.SingleLineWidth > totalWidth)) {
      this.SplitLines = this.GetSplitLines(totalWidth).ToArray();
      return (totalWidth, this.SplitLines.Length * this.SingleLineHeight);
    } else {
      return (Math.Min(this.SingleLineWidth, totalWidth), this.SingleLineHeight);
    }
  }

  private IEnumerable<string> GetSplitLines(int maxLineWidth) {
    string currentLine = "";

    foreach (string word in this.RawText.Split(' ')) {
      string possibleLine = $"{currentLine} {word}".Trim();

      if (this.MeasureSingleLine(possibleLine).X > maxLineWidth) {
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
