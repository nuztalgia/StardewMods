using StardewModdingAPI;

namespace Nuztalgia.StardewMods.Common;

internal static class Log {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value. Reviewed.
  internal static IMonitor Monitor { get; private set; }
#pragma warning restore CS8618

  internal static void Initialize(IMonitor monitor) {
    Monitor = monitor;
  }

  internal static void Error(string message) {
    Monitor.Log(message, LogLevel.Error);
  }

  internal static void Warn(string message) {
    Monitor.Log(message, LogLevel.Warn);
  }

  internal static void Info(string message) {
    Monitor.Log(message, LogLevel.Info);
  }

  internal static void Debug(string message) {
    Monitor.Log(message, LogLevel.Debug);
  }

  internal static void Trace(string message) {
#if DEBUG
    Monitor.Log(message, LogLevel.Alert);
#else
    Monitor.Log(message, LogLevel.Trace);
#endif
  }

  internal static void Verbose(string message) {
#if DEBUG
    Monitor.Log(message, LogLevel.Alert);
#else
    Monitor.VerboseLog(message);
#endif
  }
}
