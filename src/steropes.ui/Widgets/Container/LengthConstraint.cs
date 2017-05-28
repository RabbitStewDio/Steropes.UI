// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;

namespace Steropes.UI.Widgets.Container
{
  public struct LengthConstraint : IEquatable<LengthConstraint>
  {
    public enum UnitType
    {
      Auto = 0,

      Absolute = 1,

      Relative = 2
    }

    public LengthConstraint(float value, UnitType unit)
    {
      if (value <= 0 || float.IsInfinity(value) || float.IsNaN(value))
      {
        throw new ArgumentException();
      }
      Unit = unit;
      Value = value;
    }

    /// <summary>
    ///  Defines a constraint where the associated element gets exactly as much space as needed.
    /// </summary>
    public static LengthConstraint Auto => new LengthConstraint(1, UnitType.Auto);

    /// <summary>
    /// Defines a constraint where the associated element space allocated as much as needed. 
    /// Any extra space will be added relative to its associated weight and the weight of all other relative-layout elements.
    /// <param name="value">The relative weight. This must be a positive, non-zero value and cannot be infinity.</param>
    /// </summary>
    public static LengthConstraint Percentage(float value) => new LengthConstraint(value, UnitType.Relative);

    /// <summary>
    /// Defines a constraint where the associated element gets exactly the specified amount. No extra space is added even if more is available.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LengthConstraint Pixels(float value) => new LengthConstraint(value, UnitType.Absolute);

    public UnitType Unit { get; }

    public float Value { get; }

    public bool Equals(LengthConstraint other)
    {
      return Unit == other.Unit && Value.Equals(other.Value);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      return obj is LengthConstraint && Equals((LengthConstraint)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((int)Unit * 397) ^ Value.GetHashCode();
      }
    }

    public static bool operator ==(LengthConstraint left, LengthConstraint right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(LengthConstraint left, LengthConstraint right)
    {
      return !left.Equals(right);
    }

    public static LengthConstraint Absolute(int pixels)
    {
      return new LengthConstraint(pixels, UnitType.Absolute);
    }

    public static LengthConstraint Relative(int percentOfSpaceLeft)
    {
      return new LengthConstraint(percentOfSpaceLeft, UnitType.Relative);
    }
  }
}