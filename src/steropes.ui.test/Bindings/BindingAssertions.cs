using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Events;
using FluentAssertions.Execution;

namespace Steropes.UI.Test.Bindings
{
  public static class BindingAssertions
  {
    public static IEventRecorder RaisePropertyChange<T>(this EventAssertions<T> self,
                                                        object subject,
                                                        string propertyName)
    {
      return self.Raise(nameof(INotifyPropertyChanged.PropertyChanged))
        .WithSender(subject)
        .WithArgs<PropertyChangedEventArgs>(a => a.PropertyName == propertyName);
    }

    public static IEventRecorder RaiseCollectionChange<T>(this EventAssertions<T> self,
                                                          object subject,
                                                          NotifyCollectionChangedEventArgs args)
    {

      var wrappedA = new NColChngEvtArgFix(args);
      bool AssertEvent(NotifyCollectionChangedEventArgs arg)
      {
        return new NColChngEvtArgFix(arg).IsSameOrEqualTo(wrappedA);
      }
      
      var eventRecorder = self.Raise(nameof(INotifyCollectionChanged.CollectionChanged)).WithSender(subject);

      if (!eventRecorder.First().Parameters.OfType<NotifyCollectionChangedEventArgs>().Any())
      {
        throw new ArgumentException("No argument of event " + eventRecorder.EventName + " is of type <" + (object) typeof (NotifyCollectionChangedEventArgs) + ">.");
      }

      if (eventRecorder.All(recordedEvent =>
                              !recordedEvent.Parameters.OfType<NotifyCollectionChangedEventArgs>().Any(AssertEvent)))
      {
        Execute.Assertion.FailWith("Expected at least one event with arguments matching {0}, but found {1}.", args, eventRecorder.ToList());
      }
      return eventRecorder;
    }

    public static IList AsList(params object[] args)
    {
      var l = new ArrayList();
      foreach (var o in args)
      {
        l.Add(o);
      }

      return l;
    }
  }

  
    // This struct exists because the C# standard library (as usual) as 
    // glaring flaws in its design. NotifyCollectionChangedEventArgs does
    // not implement a sensible equals method, so we could not actually
    // compare events with the original implementation.
}