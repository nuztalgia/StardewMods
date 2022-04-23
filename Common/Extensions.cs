using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
}

internal static class IEnumerableExtensions {

  internal static string CommaJoin(this IEnumerable<object> items) {
    return string.Join(", ", items);
  }

  internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
    foreach (T item in items) {
      action(item);
    }
  }

  internal static void ForEach<K, V>(this IEnumerable<KeyValuePair<K, V>> items, Action<K, V> action) {
    foreach ((K key, V value) in items) {
      action(key, value);
    }
  }

  internal static void ForEach<T1, T2>(this IEnumerable<(T1, T2)> items, Action<T1, T2> action) {
    foreach ((T1 first, T2 second) in items) {
      action(first, second);
    }
  }
}

internal static class SpriteBatchExtensions {

  internal static void Draw(
      this SpriteBatch sb, Texture2D texture, Vector2 position, Rectangle sourceRect, float scale) {
    sb.Draw(texture, position, sourceRect, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale, effects: SpriteEffects.None, layerDepth: 1f);
  }
}
