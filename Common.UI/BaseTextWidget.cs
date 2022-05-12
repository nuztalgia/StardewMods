using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract class BaseTextWidget : BaseWidget {

  internal abstract class Simple : BaseTextWidget {

    protected enum FontSize { Regular, Small }

    protected override int SingleLineWidth => (int) this.MeasureSingleLine(this.Text).X;
    protected override int SingleLineHeight { get; }

    private static readonly Dictionary<FontSize, int> LineHeights = new();

    private readonly SpriteFont Font;
    private readonly bool DrawShadow;

    protected Simple(FontSize fontSize, bool drawShadow, bool wrapLines) : base(wrapLines) {
      this.Font = (fontSize == FontSize.Small) ? Game1.smallFont : Game1.dialogueFont;
      this.DrawShadow = drawShadow;

      if (!LineHeights.ContainsKey(fontSize)) {
        // This widget's line height should only depend on the font size that it specified.
        LineHeights.Add(fontSize, (int) this.MeasureSingleLine("This text is irrelevant!").Y);
      }

      this.SingleLineHeight = LineHeights[fontSize];
    }

    internal override sealed Vector2 MeasureSingleLine(string text) {
      return this.Font.MeasureString(text);
    }

    protected override sealed void Draw(SpriteBatch sb, Vector2 position, string text) {
      if (this.DrawShadow) {
        Utility.drawTextWithShadow(sb, text, this.Font, position, Game1.textColor);
      } else {
        sb.DrawString(
            this.Font, text, position, Game1.textColor,
            rotation: 0f, origin: Vector2.Zero, scale: 1f,
            effects: SpriteEffects.None, layerDepth: 1f);
      }
    }
  }

  protected abstract string Text { get; }
  protected abstract int SingleLineWidth { get; }
  protected abstract int SingleLineHeight { get; }

  private readonly bool WrapLines;

  private string[]? SplitLines;

  protected BaseTextWidget(bool wrapLines = false) {
    this.WrapLines = wrapLines;
  }

  internal abstract Vector2 MeasureSingleLine(string text);

  protected abstract void Draw(SpriteBatch sb, Vector2 position, string text);

  protected override sealed void Draw(SpriteBatch sb, Vector2 position) {
    if (this.SplitLines == null) {
      this.Draw(sb, position, this.Text);
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

    foreach (string word in this.Text.Split(' ')) {
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
