namespace Steropes.UI.Bindings
{
  public static class NumericBindings
  {
    #region Long

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<long> self,
                                                       IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<long> self,
                                                      IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<long> self,
                                                        IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<int> self,
                                                     IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<int> self,
                                                       IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<int> self,
                                                      IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<int> self,
                                                        IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<float> self,
                                                       IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a + b);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<long> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<long> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<long> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<int> self,
                                                          IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<int> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<int> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<int> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<float> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a - b);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<long> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<long> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<long> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<int> self,
                                                          IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<int> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<int> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<int> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<float> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a * b);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<long> self,
                                                          IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<long> self,
                                                         IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<long> self,
                                                           IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<int> self,
                                                        IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<int> self,
                                                          IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<int> self,
                                                         IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<int> self,
                                                           IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<float> self,
                                                          IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<float> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<double> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           IReadOnlyObservableValue<decimal> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           IReadOnlyObservableValue<long> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           IReadOnlyObservableValue<int> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           IReadOnlyObservableValue<short> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           IReadOnlyObservableValue<byte> other)
    {
      return Binding.Combine(self, other, (a, b) => a / b);
    }

    #endregion

    
    #region negate

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> other)
    {
      return other.Map(a => -a);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> other)
    {
      return other.Map(a => -a);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> other)
    {
      return other.Map(a => -a);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> other)
    {
      return other.Map(a => -a);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> other)
    {
      return other.Map(a => -a);
    }

    #endregion

  }
}