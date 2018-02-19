using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Steropes.UI.Demo
{
  /// <summary>
  ///   Draws a crosshair through the centre of the screen. This produces a quick and dirty
  ///   visual reference point to check animations and movement against.
  /// </summary>
  public class DebugOverlayRenderer : DrawableGameComponent
  {
    readonly MetricsPrinter metricsPrinter;
    readonly int sampleFrequency;
    int frameCounter;

    public DebugOverlayRenderer(Game game) : base(game)
    {
      metricsPrinter = new MetricsPrinter(game);
      sampleFrequency = 1000;
    }

    public override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      DrawDebugInfo();
    }

    void DrawDebugInfo()
    {
      frameCounter += 1;
      if (frameCounter > sampleFrequency)
      {
        Debug.WriteLine("GraphicsDevice Profiling: {0}", metricsPrinter);
        frameCounter = 0;
      }
    }

    class MetricsPrinter
    {
      readonly Game game;

      public MetricsPrinter(Game game)
      {
        this.game = game;
      }

      public override string ToString()
      {
        var metrics = game.GraphicsDevice.Metrics;
        return
          $"ClearCount={metrics.ClearCount}, DrawCount={metrics.DrawCount}, Sprites={metrics.SpriteCount}, Targets={metrics.TargetCount}";
      }
    }
  }
}