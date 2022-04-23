using System;
using System.Collections.Generic;
using System.Linq;
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

  private static readonly Random Randomizer = new();

  internal static string CommaJoin<T>(this IEnumerable<T> items) {
    return string.Join(", ", items);
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

  internal static void ForEach<K, V>(this IDictionary<K, V> items, Action<K, V> action) {
    foreach ((K key, V value) in items) {
      action(key, value);
    }
  }

  internal static bool IsTrueValue<K, V>(this IDictionary<K, V> items, K key) {
    return items.TryGetValue(key, out V? value) && (value is true);
  }
}

internal static class SpriteBatchExtensions {

  internal static void Draw(
      this SpriteBatch sb, Texture2D texture, Vector2 position, Rectangle sourceRect, float scale) {
    sb.Draw(texture, position, sourceRect, color: Color.White, rotation: 0f,
        origin: Vector2.Zero, scale, effects: SpriteEffects.None, layerDepth: 1f);
  }
}
