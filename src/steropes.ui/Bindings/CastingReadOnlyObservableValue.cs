using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  class CastingReadOnlyObservableValue<TValue> : IReadOnlyObservableValue<TValue>
  {
    readonly IReadOnlyObservableValue parent;
    TValue value;

    public CastingReadOnlyObservableValue(IReadOnlyObservableValue parent)
    {
      this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
      this.parent.PropertyChanged += OnSourceChanged;
    }

    void OnSourceChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IReadOnlyObservableValue.Value))
      {
        if (parent.Value is TValue value1)
        {
          Value = value1;
        }
        else
        {
          Value = default(TValue);
        }
      }
    }

    public void Dispose()
    {
      this.parent.PropertyChanged -= OnSourceChanged;
    }

    public IReadOnlyList<IBindingSubscription> Sources => new[] { parent };

    public event PropertyChangedEventHandler PropertyChanged;

    object IReadOnlyObservableValue.Value
    {
      get { return Value; }
    }

    public TValue Value
    {
      get { return value; }
      set
      {
        if (Equals(this.value, value))
        {
          return;
        }

        this.value = value;
        OnPropertyChanged();
      }
    }

    [NotifyPropertyChangedInvocator]
    void OnPropertyChanged([CallerMemberName] string propertyName1 = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName1));
    }

  }
}