namespace Nuztalgia.StardewMods.Common;

internal class HarmonyPatcher {

  internal interface IMethodPatcher {
    void ApplyPrefixPatch(Type methodType, string methodName);
    void ApplyPostfixPatch(Type methodType, string methodName);
    void ApplyTranspilerPatch(Type methodType, string methodName);
    void ApplyFinalizerPatch(Type methodType, string methodName);
  }

  private record MethodPatcher(Harmony Harmony, MethodInfo OriginalMethod) : IMethodPatcher {

    private enum PatchType { Prefix, Postfix, Transpiler, Finalizer }

    public void ApplyPrefixPatch(Type methodType, string methodName) {
      this.TryApplyPatch(methodType, methodName, PatchType.Prefix);
    }

    public void ApplyPostfixPatch(Type methodType, string methodName) {
      this.TryApplyPatch(methodType, methodName, PatchType.Postfix);
    }

    public void ApplyTranspilerPatch(Type methodType, string methodName) {
      this.TryApplyPatch(methodType, methodName, PatchType.Transpiler);
    }

    public void ApplyFinalizerPatch(Type methodType, string methodName) {
      this.TryApplyPatch(methodType, methodName, PatchType.Finalizer);
    }

    private void TryApplyPatch(Type methodType, string methodName, PatchType patchType) {
      try {
        HarmonyMethod harmonyMethod = new(methodType, methodName);
        this.Harmony.Patch(
            original: this.OriginalMethod,
            prefix: (patchType is PatchType.Prefix) ? harmonyMethod : null,
            postfix: (patchType is PatchType.Postfix) ? harmonyMethod : null,
            transpiler: (patchType is PatchType.Transpiler) ? harmonyMethod : null,
            finalizer: (patchType is PatchType.Finalizer) ? harmonyMethod : null);
      } catch (Exception exception) {
        Log.Error($"Failed to apply patch '{methodName}'. Technical details:\n{exception}");
      }
    }
  }

  private readonly Harmony Harmony;

  internal HarmonyPatcher(string modId) {
    this.Harmony = new Harmony(modId);
  }

  internal IMethodPatcher? ForMethod(string qualifiedClassName, string originalMethodName) {
    if (AccessTools.TypeByName(qualifiedClassName) is not Type classType) {
      Log.Trace($"Couldn't find type of class named '{qualifiedClassName}'. Skipping patch.");
      return null;
    }

    if (AccessTools.Method(classType, originalMethodName) is not MethodInfo originalMethod) {
      Log.Trace($"Couldn't find '{originalMethodName}' on '{qualifiedClassName}'. Skipping patch.");
      return null;
    }

    return new MethodPatcher(this.Harmony, originalMethod);
  }
}
