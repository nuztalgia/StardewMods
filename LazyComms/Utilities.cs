using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class Utilities {

  // Iteratively calls ExpandString() on the input enumerable.
  internal static string[] ExpandEnumerable(IEnumerable<string> input) {
    IEnumerable<string> result = new List<string>();
    return input.Aggregate(result, (result, value) => result.Concat(ExpandString(value))).ToArray();
  }

  // Recursively calls itself through ExpandEnumerable() if the input string contains multiple args.
  internal static string[] ExpandString(string input) {
    if (!Aliases.Data.TryGetValue(input, out string? value)) {
      return new string[] { input };
    }
    string[] result = ParseArgs(value);
    return result.Length == 1 ? result : ExpandEnumerable(result);
  }

  // Mostly copied from SMAPI's CommandManager.
  internal static string[] ParseArgs(string input) {
    bool inQuotes = false;
    IList<string> args = new List<string>();
    StringBuilder currentArg = new StringBuilder();

    foreach (char ch in input) {
      if (ch == '"') {
        inQuotes = !inQuotes;
      } else if (!inQuotes && char.IsWhiteSpace(ch)) {
        args.Add(currentArg.ToString());
        currentArg.Clear();
      } else {
        currentArg.Append(ch);
      }
    }
    args.Add(currentArg.ToString());
    return args.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray();
  }

  // Extension method to format a string array as a string separated by spaces.
  internal static string Stringify(this string[] components) {
    return string.Join(" ", components);
  }
}
