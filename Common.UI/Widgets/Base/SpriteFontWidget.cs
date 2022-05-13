using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using SFont = Microsoft.Xna.Framework.Graphics.SpriteFont;

namespace Nuztalgia.StardewMods.Common.UI;

internal abstract partial class Widget {

  internal abstract partial class Text : Widget {

    internal abstract class SpriteFont : Text {

      protected enum FontSize { Regular, Small }

      protected override int SingleLineWidth => (int) this.MeasureSingleLine(this.RawText).X;
      protected override int SingleLineHeight { get; }

      private static readonly Dictionary<FontSize, int> LineHeights = new();

      private readonly SFont Font;
      private readonly bool DrawShadow;

      protected SpriteFont(FontSize fontSize, Alignment? alignment, bool wrapLines, bool drawShadow)
          : base(alignment, wrapLines) {
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
  }
}
