using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Test.Bindings;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Test.UI.Widgets
{
  public class ContentWidgetTest
  {
    [Test]
    public void ChangingContentFiresPropertyChangeEvent()
    {
      var style = LayoutTestStyle.Create();
      var widget = new ContentWidget<Label>(style);

      using (var monitoredBinding = widget.Monitor<INotifyPropertyChanged>())
      {
        widget.Content = new Label(style);

        monitoredBinding.Should().RaisePropertyChange(widget, nameof(widget.Content));
        monitoredBinding.Should().RaisePropertyChange(widget, "InternalContent");
      }
    }
  }

}