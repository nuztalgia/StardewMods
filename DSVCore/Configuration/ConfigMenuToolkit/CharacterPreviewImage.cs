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

    TryDraw(sb,
        ref position,
        ref baselineAdjusted,
        this.CharacterState.PortraitRects,
        this.CharacterState.CurrentPortraits,
        PortraitScale);

    TryDraw(sb,
        ref position,
        ref baselineAdjusted,
        this.CharacterState.SpriteRects,
        this.CharacterState.CurrentSprites,
        SpriteScale);
  }

  private static void TryDraw(
      SpriteBatch sb,
      ref Vector2 position,
      ref bool baselineAdjusted,
      Rectangle[][]? allRects,
      Texture2D[][]? allImages,
      int scale) {

    if ((allRects is null) || (allImages is null)
        || (!allImages.Any()) || (!allImages.First().Any())) {
      return;
    }

    foreach (var (rectGroup, imageGroup) in allRects.Zip(allImages)) {
      if ((rectGroup.Length != 1) && (rectGroup.Length != imageGroup.Length)) {
        Log.Error($"Mismatch: {rectGroup.Length} rectangles and {imageGroup.Length} images.");
      } else {
        Rectangle mainRect = rectGroup.First();
        position.Y -= baselineAdjusted ? (mainRect.Height * scale) : 0;

        if (rectGroup.Length == 1) {
          foreach (Texture2D image in imageGroup) {
            sb.Draw(image, position, mainRect, scale);
          }
        } else if (rectGroup.Length == imageGroup.Length) {
          foreach ((Rectangle rect, Texture2D image) in rectGroup.Zip(imageGroup)) {
            sb.Draw(image, position, rect, scale);
          }
        }

        position.X += (mainRect.Width * scale) + (StandardMargin * 2);
        position.Y += mainRect.Height * scale;
        baselineAdjusted = true;
      }
    }
  }
}
