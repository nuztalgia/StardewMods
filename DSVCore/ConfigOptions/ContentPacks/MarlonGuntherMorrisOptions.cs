namespace Nuztalgia.StardewMods.DSVCore;

internal class MarlonGuntherMorrisOptions : BaseContentPackOptions {

  internal class LocalOptions {
    internal enum MorrisVariant {
      Vanilla,
      Off
    }
  }

  internal class MarlonOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public bool MermaidPendant { get; set; } = true;
  }

  internal class GuntherOptions : BaseOptions {
    public GlobalOptions.StandardVariant Variant { get; set; } =
        GlobalOptions.StandardVariant.Vanilla;
    public bool MermaidPendant { get; set; } = true;
    public bool AlternateCecily { get; set; } = true;
  }

  internal class MorrisOptions : BaseOptions {
    public LocalOptions.MorrisVariant Variant { get; set; } =
        LocalOptions.MorrisVariant.Off;
  }

  public MarlonOptions Marlon { get; set; } = new();
  public GuntherOptions Gunther { get; set; } = new();
  public MorrisOptions Morris { get; set; } = new();
}
