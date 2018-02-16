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

    public static IReadOnlyObservableValue<string> Substring(this IReadOnlyObservableValue<string> that, int start, int count)
    {
      string SafeSubstring(string source)
      {
        if (source == null)
        {
          return null;
        }

        if (source.Length < start)
        {
          return "";
        }

        if (count < 0)
        {
          count = Math.Max(0, source.Length - start);
        }

        if (count == 0)
        {
          return "";
        }

        return source.Substring(start, count);
      }

      return that.Map(SafeSubstring);
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