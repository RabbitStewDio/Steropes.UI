using System.Diagnostics;

namespace Steropes.UI.Platform
{
  public static class TracingUtil
  {
    public static readonly TraceSource InputTracing = new TraceSource("Steropes.UI.Input", SourceLevels.Warning);
    public static readonly TraceSource StyleTracing = new TraceSource("Steropes.UI.Styles", SourceLevels.Warning);

  }
}
