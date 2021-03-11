using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal class ObservableArrayBinding<T> : ReadOnlyObservableListBindingBase<T>
  {
    readonly IReadOnlyObservableValue<IReadOnlyList<T>> listValue;
    IReadOnlyList<T> data;
    int count;

    public ObservableArrayBinding(IReadOnlyObservableValue<IReadOnlyList<T>> listValue)
    {
      this.listValue = listValue;
      this.listValue.PropertyChanged += OnDataChanged;
      this.data = listValue.Value;
      this.count = data?.Count ?? 0;
    }

    public override void Dispose()
    {
      this.listValue.PropertyChanged -= OnDataChanged;
    }

    public override IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { listValue };

    void OnDataChanged(object sender, PropertyChangedEventArgs e)
    {
      data = listValue.Value;
      SetCount(data?.Count ?? 0);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ListBinding.IndexerName));
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public override int Count
    {
      get { return count; }
    }

    void SetCount(int value)
    {
      if (value != count)
      {
        count = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
      }
    }

    public override T this[int index]
    {
      get { return data[index]; }
    }

    public override event PropertyChangedEventHandler PropertyChanged;
    public override event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}