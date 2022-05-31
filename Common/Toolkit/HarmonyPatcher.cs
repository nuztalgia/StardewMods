namespace Nuztalgia.StardewMods.Common;

internal sealed class HarmonyPatcher {

  internal interface IMethodPatcher {
    void ApplyPrefixPatch(string methodName);
    void ApplyPostfixPatch(string methodName);
    void ApplyTranspilerPatch(string methodName);
    void ApplyFinalizerPatch(string methodName);
  }

  private sealed class MethodPatcher : IMethodPatcher {

    private enum PatchType { Prefix, Postfix, Transpiler, Finalizer }

    private readonly Harmony Harmony;
    private readonly Type SourceType;
    private readonly MethodInfo OriginalMethod;

    internal MethodPatcher(HarmonyPatcher harmonyPatcher, MethodInfo originalMethod) {
      this.Harmony = harmonyPatcher.Harmony;
      this.SourceType = harmonyPatcher.SourceType;
      this.OriginalMethod = originalMethod;
    }

    public void ApplyPrefixPatch(string methodName) {
      this.TryApplyPatch(methodName, PatchType.Prefix);
    }

    public void ApplyPostfixPatch(string methodName) {
      this.TryApplyPatch(methodName, PatchType.Postfix);
    }

    public void ApplyTranspilerPatch(string methodName) {
      this.TryApplyPatch(methodName, PatchType.Transpiler);
    }

    public void ApplyFinalizerPatch(string methodName) {
      this.TryApplyPatch(methodName, PatchType.Finalizer);
    }

    private void TryApplyPatch(string methodName, PatchType patchType) {
      try {
        HarmonyMethod harmonyMethod = new(this.SourceType, methodName);
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
  private readonly Type SourceType;

  internal HarmonyPatcher(string modId, Type sourceType) {
    this.Harmony = new Harmony(modId);
    this.SourceType = sourceType;
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

    return this.ForMethod(originalMethod);
  }

  internal IMethodPatcher ForMethod(MethodInfo originalMethod) {
    return new MethodPatcher(this, originalMethod);
  }
}
