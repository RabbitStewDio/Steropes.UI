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

using Steropes.UI.Annotations;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  /// <summary>
  ///   An edit-info instance that claims that the element just has been created.
  /// </summary>
  public class RebuildElementEditInfo : IElementEdit
  {
    static readonly ITextNode[] EmptyNodes = new ITextNode[0];

    public RebuildElementEditInfo(ITextNode newElement)
    {
      var e = new ITextNode[newElement.Count];
      for (var i = 0; i < e.Length; i += 1)
      {
        e[i] = newElement[i];
      }

      NewElement = newElement;
      AddedNodes = e;
    }

    public ITextNode[] AddedNodes { get; }

    public int Index => 0;

    public ITextNode NewElement { get; }

    [CanBeNull]
    public ITextNode OldElement => null;

    public ITextNode[] RemovedNodes => EmptyNodes;
  }
}