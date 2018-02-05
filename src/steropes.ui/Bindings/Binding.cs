using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Steropes.UI.Bindings
{
  public static class Binding
  {
    class Visitor : ExpressionVisitor
    {
      readonly Action<Expression> action;

      public Visitor(Action<Expression> action)
      {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
      }

      public override Expression Visit(Expression node)
      {
        action(node);
        return base.Visit(node);
      }
    }

    public static IReadOnlyObservableValue<TValue> BindingFor<TSource, TValue>(
      this TSource source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      return new ConstBinding<TSource>(source).Bind(memberExpression);
    }

    public static IReadOnlyObservableValue<TValue> Bind<TSource, TValue>(
      this IReadOnlyObservableValue<TSource> source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      string propertyName = null;
      var v = new Visitor(b =>
      {
        if (b is MemberExpression me && propertyName == null)
        {
          propertyName = me.Member.Name;
        }
      });
      v.Visit(memberExpression.Body);
      if (propertyName == null)
      {
        throw new ArgumentException("Cannot determine property name for binding.");
      }

      return new TypeSafePropertyBinding<TSource, TValue>(source, propertyName, memberExpression.Compile());
    }

    public static void BindTo<TValue>(this IReadOnlyObservableValue<TValue> value, Action<TValue> setter)
    {
      value.PropertyChanged += (s, a) => setter(value.Value);
      setter(value.Value);
    }

    public static IReadOnlyObservableValue<TValue> ChainFor<TSource, TValue>(
      this TSource source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      return new ConstBinding<TSource>(source).Chain(memberExpression);
    }

    public static IReadOnlyObservableValue<TValue> Chain<TSource, TValue>(
      this IReadOnlyObservableValue<TSource> source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      var expr = memberExpression.Body;
      var binding = CreateRecursively(expr, source);
      return binding.AsInstanceOf<TValue>();
    }

    public static IReadOnlyObservableValue<TValue> AsInstanceOf<TValue>(this IReadOnlyObservableValue source)
    {
      return new CastingReadOnlyObservableValue<TValue>(source);
    }

    public static IReadOnlyObservableValue Create<TValue>(Expression<Func<TValue>> memberExpression)
    {
      var currentExpression = memberExpression.Body;
      return CreateRecursively(currentExpression, null);
    }

    public static IReadOnlyObservableValue Map(this IReadOnlyObservableValue source, Func<object, object> mapping)
    {
      return new MonadicBinding(source, mapping);
    }

    public static IReadOnlyObservableValue<TTarget> Map<TSource, TTarget>(
      this IReadOnlyObservableValue<TSource> source,
      Func<TSource, TTarget> mapping)
    {
      return new MonadicBinding<TSource, TTarget>(source, mapping);
    }

    public static IReadOnlyObservableValue Filter(this IReadOnlyObservableValue source, Func<object, bool> filter)
    {
      return Map(source, v => filter(v) ? v : null);
    }

    public static IReadOnlyObservableValue OrElse(this IReadOnlyObservableValue source, object fallback)
    {
      return Map(source, v => v ?? fallback);
    }

    public static IReadOnlyObservableValue OrElse(this IReadOnlyObservableValue source,
                                                  IReadOnlyObservableValue fallback)
    {
      return Combine(source, fallback, (a, b) => a ?? b);
    }

    public static IReadOnlyObservableValue Combine(IReadOnlyObservableValue sourceA,
                                                   IReadOnlyObservableValue sourceB,
                                                   Func<object, object, object> mapper)
    {
      return new DelegateBoundBinding<object>(() => mapper(sourceA.Value, sourceB.Value), sourceA, sourceB);
    }

    public static IReadOnlyObservableValue Combine(IReadOnlyObservableValue sourceA,
                                                   IReadOnlyObservableValue sourceB,
                                                   IReadOnlyObservableValue sourceC,
                                                   Func<object, object, object, object> mapper)
    {
      return new DelegateBoundBinding<object>(() => mapper(sourceA.Value, sourceB.Value, sourceC.Value),
                                              sourceA, sourceB, sourceC);
    }

    public static IReadOnlyObservableValue Combine(IReadOnlyObservableValue sourceA,
                                                   IReadOnlyObservableValue sourceB,
                                                   IReadOnlyObservableValue sourceC,
                                                   IReadOnlyObservableValue sourceD,
                                                   Func<object, object, object, object, object> mapper)
    {
      return new DelegateBoundBinding<object>(() => mapper(sourceA.Value, sourceB.Value, sourceC.Value, sourceD.Value),
                                              sourceA, sourceB, sourceC, sourceD);
    }

    public static IReadOnlyObservableValue Combine(IReadOnlyObservableValue sourceA,
                                                   IReadOnlyObservableValue sourceB,
                                                   IReadOnlyObservableValue sourceC,
                                                   IReadOnlyObservableValue sourceD,
                                                   IReadOnlyObservableValue sourceE,
                                                   Func<object, object, object, object, object, object> mapper)
    {
      return new DelegateBoundBinding<object>(
        () => mapper(sourceA.Value, sourceB.Value, sourceC.Value, sourceD.Value, sourceE.Value),
        sourceA, sourceB, sourceC, sourceD, sourceE);
    }

    public static IReadOnlyObservableValue<TTarget> Combine<TSourceA, TSourceB, TTarget>(
      IReadOnlyObservableValue<TSourceA> sourceA,
      IReadOnlyObservableValue<TSourceB> sourceB,
      Func<TSourceA, TSourceB, TTarget> mapper)
    {
      return new DelegateBoundBinding<TTarget>(() => mapper(sourceA.Value, sourceB.Value), sourceA, sourceB);
    }

    public static IReadOnlyObservableValue<TTarget> Combine<TSourceA, TSourceB, TSourceC, TTarget>(
      IReadOnlyObservableValue<TSourceA> sourceA,
      IReadOnlyObservableValue<TSourceB> sourceB,
      IReadOnlyObservableValue<TSourceC> sourceC,
      Func<TSourceA, TSourceB, TSourceC, TTarget> mapper)
    {
      return new DelegateBoundBinding<TTarget>(() => mapper(sourceA.Value, sourceB.Value, sourceC.Value),
                                               sourceA, sourceB, sourceC);
    }

    public static IReadOnlyObservableValue<TTarget> Combine<TSourceA, TSourceB, TSourceC, TSourceD, TTarget>(
      IReadOnlyObservableValue<TSourceA> sourceA,
      IReadOnlyObservableValue<TSourceB> sourceB,
      IReadOnlyObservableValue<TSourceC> sourceC,
      IReadOnlyObservableValue<TSourceD> sourceD,
      Func<TSourceA, TSourceB, TSourceC, TSourceD, TTarget> mapper)
    {
      return new DelegateBoundBinding<TTarget>(() => mapper(sourceA.Value, sourceB.Value, sourceC.Value, sourceD.Value),
                                               sourceA, sourceB, sourceC, sourceD);
    }

    public static IReadOnlyObservableValue<TTarget> Combine<TSourceA, TSourceB, TSourceC, TSourceD, TSourceE, TTarget>(
      IReadOnlyObservableValue<TSourceA> sourceA,
      IReadOnlyObservableValue<TSourceB> sourceB,
      IReadOnlyObservableValue<TSourceC> sourceC,
      IReadOnlyObservableValue<TSourceD> sourceD,
      IReadOnlyObservableValue<TSourceE> sourceE,
      Func<TSourceA, TSourceB, TSourceC, TSourceD, TSourceE, TTarget> mapper)
    {
      return new DelegateBoundBinding<TTarget>(
        () => mapper(sourceA.Value, sourceB.Value, sourceC.Value, sourceD.Value, sourceE.Value),
        sourceA, sourceB, sourceC, sourceD, sourceE);
    }

    static IReadOnlyObservableValue CreateRecursively(Expression currentExpression, IReadOnlyObservableValue source)
    {
      if (currentExpression is ConstantExpression constExpr)
      {
        return new ConstBinding(constExpr.Value);
      }

      if (currentExpression is ParameterExpression)
      {
        if (source == null)
        {
          throw new ArgumentException();
        }

        return source;
      }

      if (!(currentExpression is MemberExpression memberExpr))
      {
        throw new ArgumentException($"Require member expression, found WHATEVER in lamda.");
      }

      var obs = CreateRecursively(memberExpr.Expression, source);
      var member = memberExpr.Member;
      if (member.MemberType == MemberTypes.Field)
      {
        return new PropertyMemberBinding(obs, (FieldInfo) member);
      }

      if (member.MemberType == MemberTypes.Property)
      {
        return new PropertyMemberBinding(obs, (PropertyInfo) member);
      }

      throw new ArgumentException("Cannot have expression of type " + currentExpression.NodeType + " in call chain.");
    }
  }
}