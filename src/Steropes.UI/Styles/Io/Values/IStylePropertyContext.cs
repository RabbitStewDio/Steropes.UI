using Steropes.UI.Platform;

namespace Steropes.UI.Styles.Io.Values
{
  public interface IStylePropertyContext
  {
    IUITexture ProcessTexture(IUITexture texture);
    IBoxTexture ProcessTexture(IBoxTexture texture);
  }
}