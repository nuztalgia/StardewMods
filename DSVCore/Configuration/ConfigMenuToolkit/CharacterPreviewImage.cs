using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;

namespace Nuztalgia.StardewMods.DSVCore;

internal class CharacterPreviewImage {

  private const int PortraitScale = 3;
  private const int SpriteScale = 5;
  private const int StandardMargin = 16;
  private const int MinimumHeight = 128;

  private readonly CharacterConfigState CharacterState;

  internal CharacterPreviewImage(CharacterConfigState characterState) {
    this.CharacterState = characterState;
  }

  internal int GetHeight() {
    int actualHeight = Math.Max(
        GetHeight(this.CharacterState.PortraitRects, PortraitScale),
        GetHeight(this.CharacterState.SpriteRects, SpriteScale));

    return Math.Max(actualHeight, MinimumHeight) + StandardMargin;

    static int GetHeight(Rectangle[][]? rects, int scale) {
      return (rects is not null) ? rects[0][0].Height * scale : 0;
    }
  }

  internal void Draw(SpriteBatch sb, Vector2 position) {
    position.Y += StandardMargin;
    bool baselineAdjusted = false;

    TryDraw(this.CharacterState.PortraitRects, this.CharacterState.CurrentPortraits, PortraitScale);
    TryDraw(this.CharacterState.SpriteRects, this.CharacterState.CurrentSprites, SpriteScale);

    void TryDraw(Rectangle[][]? allRects, Texture2D[][]? allImages, int scale) {
      if ((allRects is null) || (allImages is null)
          || allImages.IsEmpty() || allImages.First().IsEmpty()) {
        return;
      }

      foreach (var (rectGroup, imageGroup) in allRects.Zip(allImages)) {
        if ((rectGroup.Length != 1) && (rectGroup.Length != imageGroup.Length)) {
          Log.Error($"Mismatch: {rectGroup.Length} rectangles and {imageGroup.Length} images.");
        } else {
          Rectangle mainRect = rectGroup.First();
          position.Y -= baselineAdjusted ? (mainRect.Height * scale) : 0;

          if (rectGroup.Length == 1) {
            imageGroup.ForEach((Texture2D image) => sb.Draw(image, position, mainRect, scale));
          } else if (rectGroup.Length == imageGroup.Length) {
            rectGroup.Zip(imageGroup).ForEach(
                (Rectangle rect, Texture2D image) => sb.Draw(image, position, rect, scale));
          }

          position.X += (mainRect.Width * scale) + (StandardMargin * 2);
          position.Y += mainRect.Height * scale;
          baselineAdjusted = true;
        }
      }
    }
  }
}
