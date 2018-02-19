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

    /// <summary>
    ///  Returns a binding that creates a direct binding from the source object to the property 
    ///  named in the ``propertyName`` parameter. The value for the property is extracted using 
    ///  the provided delegate function.
    /// <para/>
    ///  Note: This method will create a single binding segment. If you want to bind a chain of 
    ///  property accessors use the <see cref="ChainFor{TSource,TValue}"/> method instead.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="getter"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> BindingFor<TSource, TValue>(this TSource source, string propertyName, Func<TSource, TValue> getter)
    {
      return new TypeSafePropertyBinding<TSource, TValue>(new ConstBinding<TSource>(source), propertyName, getter);
    }


    /// <summary>
    ///  Returns a binding for the property accessed by the given expression. This method attempts
    ///  to parse the expression tree to extract the name of the property that should be monitored.
    ///  It will select the first property or field access it finds as property name for monitoring.
    ///  <para/>
    ///  Although it is possible to perform basic arithmetics or even function calls here, I 
    ///  recommend that such operations are performed in a separate <see cref="Map{TSource,TTarget}"/>
    ///  call instead. This will create bindings that express the intent of these operations more 
    ///  clearly and will yield more stable bindings that are resilient to refactorings or other 
    ///  code changes.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> BindingFor<TSource, TValue>(
      this TSource source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      return new ConstBinding<TSource>(source).Bind(memberExpression);
    }

    /// <summary>
    ///  Creates a continuation binding that will bind to the value contained in the 
    ///  ``IReadOnlyObservableValue`` provided as source using the given member expression as 
    ///   extractor function. The binding will reevaluate when either the source-binding's
    ///   value changes or when the contained value signals a property change.
    /// <para/>
    ///  Note: This does not create a chained binding.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> Bind<TSource, TValue>(this IReadOnlyObservableValue<TSource> source,
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

      return Bind(source, propertyName, memberExpression.Compile());
    }

    /// <summary>
    ///  Creates a continuation binding that will bind to the value contained in the 
    ///  ``IReadOnlyObservableValue`` provided as source using the given extractor function. 
    ///   The binding will reevaluate when either the source-binding's value changes or when 
    ///   the contained value signals a property change for the property named in the
    ///   ``propertyName`` parameter.
    /// <para/>
    ///  Note: This does not create a chained binding.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> Bind<TSource, TValue>(this IReadOnlyObservableValue<TSource> source,
                                                                         string propertyName, 
                                                                         Func<TSource, TValue> memberExpression)
    {
      if (propertyName == null)
      {
        throw new ArgumentException("Cannot determine property name for binding.");
      }

      return new TypeSafePropertyBinding<TSource, TValue>(source, propertyName, memberExpression);
    }

    /// <summary>
    ///  Binds the given observable value to the action provided. Use this to apply a bound value to
    /// its target object.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <param name="setter"></param>
    public static void BindTo<TValue>(this IReadOnlyObservableValue<TValue> value, Action<TValue> setter)
    {
      value.PropertyChanged += (s, a) => setter(value.Value);
      setter(value.Value);
    }

    /// <summary>
    ///  Create a chained binding for the properties accessed in the member expression. This will
    ///  create a binding for each member access in the given expression and the resulting binding
    ///  will change when either segment or the source signals a property change notification.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> ChainFor<TSource, TValue>(
      this TSource source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      return new ConstBinding<TSource>(source).Chain(memberExpression);
    }

    /// <summary>
    ///  Create a chained continuation binding for the properties accessed in the member expression. 
    ///  This will create a binding for each member access in the given expression and the resulting 
    ///  binding will change when either segment or the source signals a property change notification.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> Chain<TSource, TValue>(
      this IReadOnlyObservableValue<TSource> source,
      Expression<Func<TSource, TValue>> memberExpression)
    {
      var expr = memberExpression.Body;
      var binding = CreateRecursively(expr, source);
      return binding.AsInstanceOf<TValue>();
    }

    /// <summary>
    ///  Provides a safe cast operation on the binding value. The binding will evaluate either to
    ///  a valid TValue instance or the built-in default value (ie null for reference objects).
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue<TValue> AsInstanceOf<TValue>(this IReadOnlyObservableValue source, TValue defaultValue = default(TValue))
    {
      return new CastingReadOnlyObservableValue<TValue>(source, defaultValue);
    }

    /// <summary>
    ///  Creates a chained binding for the given expression. This method will try to 
    ///  use the first member access as source of the binding chain. This first member access
    ///  may be a implicit ```this`` or a member access to an automatically generated closure
    ///  object. If this method results in unpredictable results, switch to the alternative
    ///  <see cref="ChainFor{TSource,TValue}"/> or 
    ///  <see cref="BindingFor{TSource,TValue}(TSource,System.Linq.Expressions.Expression{System.Func{TSource,TValue}})"/>
    ///  methods instead.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static IReadOnlyObservableValue Create<TValue>(Expression<Func<TValue>> memberExpression)
    {
      var currentExpression = memberExpression.Body;
      return CreateRecursively(currentExpression, null);
    }

    public static IReadOnlyObservableValue Map(this IReadOnlyObservableValue source, Func<object, object> mapping)
    {
      return new TransformingBinding(source, mapping);
    }

    public static IReadOnlyObservableValue<TTarget> Map<TSource, TTarget>(
      this IReadOnlyObservableValue<TSource> source,
      Func<TSource, TTarget> mapping)
    {
      return new TransformingBinding<TSource, TTarget>(source, mapping);
    }

    public static IReadOnlyObservableValue Filter(this IReadOnlyObservableValue source, Func<object, bool> filter, object defaultValue = null)
    {
      return source.Map(v => filter(v) ? v : defaultValue);
    }

    public static IReadOnlyObservableValue Filter<T>(this IReadOnlyObservableValue<T> source, Func<T, bool> filter, T defaultValue = default(T))
    {
      return source.Map(v => filter(v) ? v : defaultValue);
    }

    public static IReadOnlyObservableValue OrElse<T>(this IReadOnlyObservableValue<T> source, T fallback) where T: class
    {
      return source.Map(v => v ?? fallback);
    }

    public static IReadOnlyObservableValue<T> OrElse<T>(this IReadOnlyObservableValue<T> source,
                                                        IReadOnlyObservableValue<T> fallback) where T: class
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

    public static void Unbind(this IBindingSubscription sub)
    {
      sub?.Dispose();
    }

    public static void UnbindChain(this IBindingSubscription sub)
    {
      if (sub == null)
      {
        return;
      }

      sub.Dispose();
      foreach (var source in sub.Sources)
      {
        source.UnbindChain();
      }
    }
  }
}