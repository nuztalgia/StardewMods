using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuztalgia.StardewMods.LazyComms;

internal static class InputHelper {

  // Un-aliases any recognized aliases in the input string. Works for nested aliases too.
  internal static string Translate(string input) {
    return ExpandEnumerable(ParseArgs(input)).SpaceJoin();
  }

  // Iteratively calls ExpandString() on the input enumerable.
  internal static IEnumerable<string> ExpandEnumerable(IEnumerable<string> input) {
    IEnumerable<string> result = new List<string>();
    return input.Aggregate(result, (result, value) => result.Concat(ExpandString(value)));
  }

  // Recursively calls itself through ExpandEnumerable() if the input string contains multiple args.
  internal static IEnumerable<string> ExpandString(string input) {
    if (ConfigHelper.GetAliasValue(input) is string value) {
      IEnumerable<string> result = ParseArgs(value);
      return (result.Count() == 1) ? result : ExpandEnumerable(result);
    } else {
      return new string[] { input };
    }
  }

  // Adapted from SMAPI's CommandManager to ensure consistency.
  private static IEnumerable<string> ParseArgs(string input) {
    List<string> argList = new();
    StringBuilder currentArg = new();
    bool inQuotes = false;

    foreach (char character in input) {
      if (character == '"') {
        inQuotes = !inQuotes;
      } else if (!inQuotes && char.IsWhiteSpace(character)) {
        argList.Add(currentArg.ToString());
        currentArg.Clear();
      } else {
        currentArg.Append(character);
      }
    }
    argList.Add(currentArg.ToString());
    return argList.Where(arg => !string.IsNullOrWhiteSpace(arg));
  }
}
