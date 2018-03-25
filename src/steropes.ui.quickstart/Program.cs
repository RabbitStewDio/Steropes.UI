// Steopes-UI quickstart by Thomas Morgner and others
// 
// To the extent possible under law, the person who associated CC0 with the
// Steropes-UI quickstart module has waived all copyright and related or neighboring rights
// to the Steropes-UI quickstart module. 
//
// You should have received a copy of the CC0 legalcode along with this
// work.If not, see<http://creativecommons.org/publicdomain/zero/1.0/>.

namespace Steropes.UI.Quickstart
{
  class Program
  {
    public static void Main(string[] args)
    {
      using (var game = new SimpleGame())
      {
        game.Run();
      }
    }
  }
}