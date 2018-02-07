using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  internal class TypeSafePropertyBinding<TSource, TValue> : IReadOnlyObservableValue<TValue>
  {
    readonly Func<TSource, TValue> extractor;
    readonly string name;
    readonly IReadOnlyObservableValue<TSource> sourceBinding;
    TSource source;
    TValue value;

    public TypeSafePropertyBinding(IReadOnlyObservableValue<TSource> sourceBinding,
                                   string name,
                                   Func<TSource, TValue> extractor)
    {
      this.sourceBinding = sourceBinding ?? throw new ArgumentNullException();
      this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
      this.name = name;
      if (sourceBinding is INotifyPropertyChanged nc)
      {
        nc.PropertyChanged += OnSourceBindingChanged;
      }

      Source = sourceBinding.Value;
    }

    public void Dispose()
    {
      this.sourceBinding.PropertyChanged -= OnSourceBindingChanged;
      Value = default(TValue);
    }

    public IReadOnlyList<IBindingSubscription> Sources => new[] { sourceBinding };

    TSource Source
    {
      get { return source; }
      set
      {
        if (Equals(value, source))
        {
          return;
        }

        if (source is INotifyPropertyChanged onc)
        {
          onc.PropertyChanged -= OnSourcePropertyChanged;
        }

        source = value;
        if (source is INotifyPropertyChanged nnc)
        {
          nnc.PropertyChanged += OnSourcePropertyChanged;
        }

        Value = extractor(value);
      }
    }

    object IReadOnlyObservableValue.Value
    {
      get { return Value; }
    }

    public TValue Value
    {
      get { return value; }
      private set
      {
        if (Equals(value, this.value))
        {
          return;
        }

        this.value = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    void OnSourceBindingChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IReadOnlyObservableValue.Value))
      {
        Source = sourceBinding.Value;
      }
    }

    void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == name)
      {
        Value = extractor(Source);
      }
    }

    [NotifyPropertyChangedInvocator]
    void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}