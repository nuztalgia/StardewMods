using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Nuztalgia.StardewMods.Common;

internal static class StringExtensions {

  internal static bool IsEmpty(this string s) {
    return s.Length == 0;
  }

  internal static string Format(this string s, params object[] args) {
    return (args.Length == 0) ? s : string.Format(s, args);
  }

  internal static string CapitalizeFirstChar(this string s) {
    return s.IsEmpty() ? string.Empty : string.Concat(s[0].ToString().ToUpper(), s.AsSpan(1));
  }

  internal static string[] CommaSplit(this string s) {
    return s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
  }

  internal static TEnum? AsEnum<TEnum>(this string s) where TEnum : struct, Enum {
    return (s.AsEnum(typeof(TEnum)) is TEnum enumValue) ? enumValue : null;
  }

  internal static Enum? AsEnum(this string s, Type type) {
    return (Enum.TryParse(type, s, ignoreCase: true, out object? parsedValue)
            && (parsedValue is Enum) && (parsedValue?.GetType() == type))
        ? (parsedValue as Enum)
        : null;
  }
}

internal static class IEnumerableExtensions {

  private static readonly Random Randomizer = new();

  internal static bool IsEmpty<T>(this IEnumerable<T> items) {
    return !items.Any();
  }

  internal static string CommaJoin<T>(this IEnumerable<T> items) {
    return string.Join(", ", items);
  }

  internal static string SpaceJoin<T>(this IEnumerable<T> items) {
    return string.Join(' ', items);
  }

  internal static T? GetRandom<T>(this IEnumerable<T> items) {
    return items.Count() switch {
      0 => default,
      1 => items.First(),
      _ => items.ElementAt(Randomizer.Next(items.Count())),
    };
  }

  internal static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) {
    return items.OrderBy(item => Randomizer.Next());
  }

  internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
    foreach (T item in items) {
      action(item);
    }
  }

  internal static void ForEach<T1, T2>(this IEnumerable<(T1, T2)> items, Action<T1, T2> action) {
    foreach ((T1 first, T2 second) in items) {
      action(first, second);
    }
  }
}

internal static class IDictionaryExtensions {

  internal static void ForEach<K, V>(this IDictionary<K, V> dict, Action<K, V> action) {
    foreach ((K key, V value) in dict) {
      action(key, value);
    }
  }

  internal static bool IsTrueValue<K, V>(this IDictionary<K, V> dict, K key) {
    return dict.TryGetValue(key, out V? value) && (value is true);
  }

  internal static bool IsFalseValue<K, V>(this IDictionary<K, V> dict, K key) {
    return dict.TryGetValue(key, out V? value) && (value is false);
  }

  internal static bool TryGetEnumValue<E>(
      this IDictionary<string, object?> dict, string key, out E? enumValue) where E : struct, Enum {
    enumValue = dict.TryGetValue(key, out object? dictValue)
        ? dictValue?.ToString()?.AsEnum<E>()
        : null;
    return enumValue is not null;
  }
}

internal static class SpriteBatchExtensions {

  internal static void Draw(
      this SpriteBatch sb,
      Texture2D texture,
      Vector2 position,
      Rectangle sourceRect,
      float scale = Game1.pixelZoom) {

    sb.Draw(
        texture, position, sourceRect, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale, effects: SpriteEffects.None, layerDepth: 1f);
  }

  internal static void DrawTextureBox(
      this SpriteBatch sb,
      Texture2D texture,
      Vector2 position,
      Rectangle sourceRect,
      int? width = null,
      int? height = null,
      float scale = Game1.pixelZoom,
      bool drawShadow = false) {

    IClickableMenu.drawTextureBox(
        sb, texture, sourceRect, (int) position.X, (int) position.Y,
        width ?? sourceRect.Width, height ?? sourceRect.Height,
        color: Color.White, scale, drawShadow);
  }

  internal static void DrawString(
      this SpriteBatch sb,
      SpriteFont spriteFont,
      string text,
      Vector2 position,
      Color? color = null,
      float? scale = null,
      bool? drawShadow = null) {

    color ??= Game1.textColor;
    drawShadow ??= (spriteFont == Game1.dialogueFont);

    if (drawShadow is true) {
      Utility.drawTextWithShadow(sb, text, spriteFont, position, (Color) color);
    } else {
      sb.DrawString(
          spriteFont, text, position, (Color) color, rotation: 0f,
          origin: Vector2.Zero, scale ?? 1f, SpriteEffects.None, layerDepth: 1f);
    }
  }
}

internal static class SpriteFontExtensions {

  internal static int MeasureLineHeight(this SpriteFont spriteFont) {
    return (int) spriteFont.MeasureString("_").Y;
  }

  internal static IEnumerable<string> GetLines(
      this SpriteFont spriteFont, string text, float startX, float endX) {
    string currentLine = "";

    foreach (string word in text.Split(' ')) {
      string possibleLine = $"{currentLine} {word}".Trim();

      if ((startX + spriteFont.MeasureString(possibleLine).X) > endX) {
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
