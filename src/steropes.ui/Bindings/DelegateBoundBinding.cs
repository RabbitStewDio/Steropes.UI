using System;
using System.Collections.Generic;

namespace Steropes.UI.Bindings
{
  internal class DelegateBoundBinding<T> : DerivedBinding<T>
  {
    readonly Func<T> computation;
    readonly IReadOnlyObservableValue[] sources;

    public DelegateBoundBinding(Func<T> computation, params IReadOnlyObservableValue[] sources)
    {
      this.computation = computation ?? throw new ArgumentNullException(nameof(computation));
      this.sources = sources;
      foreach (var source in sources)
      {
        source.PropertyChanged += OnSourcePropertyChange;
      }
    }

    public override void Dispose()
    {
      foreach (var source in sources)
      {
        source.PropertyChanged -= OnSourcePropertyChange;
      }
    }

    public override IReadOnlyList<IBindingSubscription> Sources => sources;

    protected override T ComputeValue()
    {
      return computation();
    }
  }
}