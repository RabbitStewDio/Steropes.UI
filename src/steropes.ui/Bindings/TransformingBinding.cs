using System;
using System.Collections.Generic;

namespace Steropes.UI.Bindings
{
  internal class TransformingBinding : DerivedBinding<object>
  {
    readonly IReadOnlyObservableValue source;
    readonly Func<object, object> mapping;

    public TransformingBinding(IReadOnlyObservableValue source, Func<object, object> mapping)
    {
      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
      this.source.PropertyChanged += OnSourcePropertyChange;
      Value = mapping(source.Value);
    }

    protected override object ComputeValue()
    {
      return mapping(source.Value);
    }

    public override void Dispose()
    {
      this.source.PropertyChanged -= OnSourcePropertyChange;
    }

    public override IReadOnlyList<IBindingSubscription> Sources => new[] { source };
  }

  internal class TransformingBinding<TSource, TTarget> : DerivedBinding<TTarget>
  {
    readonly IReadOnlyObservableValue<TSource> source;
    readonly Func<TSource, TTarget> mapping;

    public TransformingBinding(IReadOnlyObservableValue<TSource> source, Func<TSource, TTarget> mapping)
    {
      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.source = source;
      this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
      this.source.PropertyChanged += OnSourcePropertyChange;
      Value = mapping(source.Value);
    }

    protected override TTarget ComputeValue()
    {
      return mapping(source.Value);
    }

    public override void Dispose()
    {
      this.source.PropertyChanged -= OnSourcePropertyChange;
    }

    public override IReadOnlyList<IBindingSubscription> Sources => new[] { source };
  }
}