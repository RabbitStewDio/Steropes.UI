// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using Steropes.UI.Components;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Demo.Demos
{
  internal class TextAreaPane : ScrollPanel
  {
    const string LoremIpsum =
      @"Lorem ipsum dolor sit amet, harum torquatos intellegat pri ex, ex everti tritani nusquam mea. Pri euripidis posidonium ne, unum natum movet sea ea. Volumus praesent inciderint in eam. Admodum deseruisse eum id, sint lobortis vis ea.

Per magna nihil meliore te, vix vero exerci nullam ut, ea vocent dolores deleniti est. Stet suscipit definitionem pro te, id soleat singulis deterruisset pro. Cu officiis appellantur concludaturque ius, vis ea rebum postea lobortis, quo te nusquam indoctum. Ius cu torquatos interesset, ferri iriure nam no. Duo ea dicat cetero urbanitas. Est ut dicant inimicus, simul nobis sed eu.

Ex vel essent aperiri sapientem, summo offendit in eos. Nam at reque doctus aliquando. Ex commodo expetendis appellantur mei. Dicta possit vivendum ne vel. Officiis explicari theophrastus id eam, et soleat detracto his, id vim dolor numquam honestatis.

Ne pri congue impedit. Id has illum omittantur signiferumque, an usu cibo euripidis adversarium. Vero blandit periculis ius et. Ei equidem ullamcorper eos, consequat abhorreant philosophia duo ea, an sonet tibique hendrerit pro.

Pro in labore tritani. Vel cu fugit oportere, omnium vocent sit ex. An mei consul ornatus. Dicat homero ex mea, pri at altera dolorum evertitur. Et vide corrumpit patrioque nec, eum no nostrud minimum, vis ut noster mollis malorum. Posse urbanitas ex duo, sed ut eius solum dicam.";

    public TextAreaPane(IUIStyle style) : base(style)
    {
      VerticalScrollbarMode = ScrollbarMode.Always;
      Content = new TextArea(UIStyle)
      {
        Text = LoremIpsum + "\n\n" + LoremIpsum, 
        Anchor = AnchoredRect.Full
      };
    }
  }
}