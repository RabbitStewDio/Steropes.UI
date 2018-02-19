namespace Steropes.UI.Bindings
{
  public static class StaticNumericBindings
  {
    #region Long

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     long other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     int other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     short other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<long> self,
                                                     byte other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<long> self,
                                                       double other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<long> self,
                                                      float other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<long> self,
                                                        decimal other)
    {
      return self.Map((a) => a + other);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Add(this IReadOnlyObservableValue<int> self,
                                                     long other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    int other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    short other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<int> Add(this IReadOnlyObservableValue<int> self,
                                                    byte other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<int> self,
                                                       double other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<int> self,
                                                      float other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<int> self,
                                                        decimal other)
    {
      return self.Map((a) => a + other);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      float other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<float> self,
                                                       double other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      long other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      int other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      short other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<float> Add(this IReadOnlyObservableValue<float> self,
                                                      byte other)
    {
      return self.Map((a) => a + other);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       float other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       double other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       long other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       int other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       short other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<double> Add(this IReadOnlyObservableValue<double> self,
                                                       byte other)
    {
      return self.Map((a) => a + other);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        decimal other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        long other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        int other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        short other)
    {
      return self.Map((a) => a + other);
    }

    public static IReadOnlyObservableValue<decimal> Add(this IReadOnlyObservableValue<decimal> self,
                                                        byte other)
    {
      return self.Map((a) => a + other);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          long other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          int other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          short other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<long> self,
                                                          byte other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<long> self,
                                                            double other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<long> self,
                                                           float other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<long> self,
                                                             decimal other)
    {
      return self.Map((a) => a - other);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Subtract(this IReadOnlyObservableValue<int> self,
                                                          long other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         int other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         short other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<int> Subtract(this IReadOnlyObservableValue<int> self,
                                                         byte other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<int> self,
                                                            double other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<int> self,
                                                           float other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<int> self,
                                                             decimal other)
    {
      return self.Map((a) => a - other);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           float other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<float> self,
                                                            double other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           long other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           int other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           short other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<float> Subtract(this IReadOnlyObservableValue<float> self,
                                                           byte other)
    {
      return self.Map((a) => a - other);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            float other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            double other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            long other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            int other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            short other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<double> Subtract(this IReadOnlyObservableValue<double> self,
                                                            byte other)
    {
      return self.Map((a) => a - other);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             decimal other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             long other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             int other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             short other)
    {
      return self.Map((a) => a - other);
    }

    public static IReadOnlyObservableValue<decimal> Subtract(this IReadOnlyObservableValue<decimal> self,
                                                             byte other)
    {
      return self.Map((a) => a - other);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          long other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          int other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          short other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<long> self,
                                                          byte other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<long> self,
                                                            double other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<long> self,
                                                           float other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<long> self,
                                                             decimal other)
    {
      return self.Map((a) => a * other);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Multiply(this IReadOnlyObservableValue<int> self,
                                                          long other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         int other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         short other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<int> Multiply(this IReadOnlyObservableValue<int> self,
                                                         byte other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<int> self,
                                                            double other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<int> self,
                                                           float other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<int> self,
                                                             decimal other)
    {
      return self.Map((a) => a * other);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           float other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<float> self,
                                                            double other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           long other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           int other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           short other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<float> Multiply(this IReadOnlyObservableValue<float> self,
                                                           byte other)
    {
      return self.Map((a) => a * other);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            float other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            double other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            long other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            int other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            short other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<double> Multiply(this IReadOnlyObservableValue<double> self,
                                                            byte other)
    {
      return self.Map((a) => a * other);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             decimal other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             long other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             int other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             short other)
    {
      return self.Map((a) => a * other);
    }

    public static IReadOnlyObservableValue<decimal> Multiply(this IReadOnlyObservableValue<decimal> self,
                                                             byte other)
    {
      return self.Map((a) => a * other);
    }

    #endregion

    #region Long

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        long other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        int other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        short other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<long> self,
                                                        byte other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<long> self,
                                                          double other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<long> self,
                                                         float other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<long> self,
                                                           decimal other)
    {
      return self.Map((a) => a / other);
    }

    #endregion

    #region Int

    public static IReadOnlyObservableValue<long> Divide(this IReadOnlyObservableValue<int> self,
                                                        long other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       int other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       short other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<int> Divide(this IReadOnlyObservableValue<int> self,
                                                       byte other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<int> self,
                                                          double other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<int> self,
                                                         float other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<int> self,
                                                           decimal other)
    {
      return self.Map((a) => a / other);
    }

    #endregion

    #region float

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         float other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<float> self,
                                                          double other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         long other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         int other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         short other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<float> Divide(this IReadOnlyObservableValue<float> self,
                                                         byte other)
    {
      return self.Map((a) => a / other);
    }

    #endregion

    #region double

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          float other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          double other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          long other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          int other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          short other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<double> Divide(this IReadOnlyObservableValue<double> self,
                                                          byte other)
    {
      return self.Map((a) => a / other);
    }

    #endregion

    #region decimal

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           decimal other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           long other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           int other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           short other)
    {
      return self.Map((a) => a / other);
    }

    public static IReadOnlyObservableValue<decimal> Divide(this IReadOnlyObservableValue<decimal> self,
                                                           byte other)
    {
      return self.Map((a) => a / other);
    }

    #endregion
  }
}