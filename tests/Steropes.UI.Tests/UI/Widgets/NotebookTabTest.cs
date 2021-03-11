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
using FluentAssertions;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class NotebookTabTest
  {
    [Test]
    public void CloseButton_Ignores_Close_Request_when_pinned()
    {
      var closeRequested = false;
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.IsPinned = true;
      tab.CloseRequested += (s, e) => closeRequested = true;
      tab[0][0].DispatchMouseClick(MouseButton.Left, 10, 20);

      closeRequested.Should().Be(false);
    }

    [Test]
    public void CloseButton_Triggers_Close_Request()
    {
      var closeRequested = false;
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.CloseRequested += (s, e) => closeRequested = true;
      tab[0][0].DispatchMouseClick(MouseButton.Left, 10, 20);

      closeRequested.Should().Be(true);
    }

    [Test]
    public void IsPinned_Hides_CloseButton()
    {
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.IsClosable = false;

      tab.IsClosable.Should().Be(false);
      tab.IsPinned.Should().Be(true);
      tab[0][0].Should().BeAssignableTo<Button>();
      tab[0][0].Visibility.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void IsPinned_InitialState()
    {
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.IsClosable.Should().Be(true);
      tab.IsPinned.Should().Be(false);

      tab[0][0].Should().BeAssignableTo<Button>();
      tab[0][0].Visibility.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Mouse_Clicks_Activate_Tab()
    {
      var activationRequested = false;
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.ActivationRequested += (s, e) => activationRequested = true;
      tab.DispatchMouseClick(MouseButton.Left, 10, 20);

      activationRequested.Should().Be(true);
    }

    [Test]
    public void Mouse_Clicks_in_label_Activate_Tab()
    {
      var activationRequested = false;
      var tab = new NotebookTab(LayoutTestStyle.Create(), LayoutTestWidget.FixedSize(100, 20), LayoutTestWidget.FixedSize(500, 400));
      tab.ActivationRequested += (s, e) => activationRequested = true;
      tab[0][1].DispatchMouseClick(MouseButton.Left, 10, 20);

      activationRequested.Should().Be(true);
    }
  }
}