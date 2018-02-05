using System;

namespace Steropes.UI.Bindings
{
  public static class StringBindings
  {
    public static IReadOnlyObservableValue<bool> IsNullOrEmpty(this IReadOnlyObservableValue<string> that)
    {
      return that.Map(string.IsNullOrEmpty);
    }

    public static IReadOnlyObservableValue<bool> IsNullOrWhitespace(this IReadOnlyObservableValue<string> that)
    {
      return that.Map(string.IsNullOrWhiteSpace);
    }

    public static IReadOnlyObservableValue<int> Length(this IReadOnlyObservableValue<string> that)
    {
      return that.Map(b => b.Length);
    }

    public static IReadOnlyObservableValue<bool> IsEqualTo(this IReadOnlyObservableValue<string> that,
                                                           string text,
                                                           StringComparison c = StringComparison.InvariantCulture)
    {
      return that.Map(b => string.Equals(b, text, c));
    }

    public static IReadOnlyObservableValue<string> Format(string format, params IReadOnlyObservableValue[] p)
    {
      p = (IReadOnlyObservableValue[]) p.Clone();

      string DoFormat()
      {
        var args = new object[p.Length];
        for (var i = 0; i < p.Length; i++)
        {
          args[i] = p[i].Value;
        }

        return string.Format(format, args);
      }

      return new DelegateBoundBinding<string>(DoFormat, p);
    }
  }
}