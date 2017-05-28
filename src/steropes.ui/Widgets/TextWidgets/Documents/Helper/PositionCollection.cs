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
using System.Collections.Generic;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Helper
{
  public class PositionCollection
  {
    readonly List<HardPosition> positions;

    public PositionCollection()
    {
      positions = new List<HardPosition>();
    }

    public IEnumerable<ITextPosition> Contents()
    {
      for (var index = 0; index < positions.Count; index++)
      {
        var position = positions[index];
        yield return new TextPosition(position.Offset, position.Bias);
      }
    }

    public ITextPosition Create(int offset, Bias bias)
    {
      PruneObsolete();

      var insertPosition = FindInsertPosition(offset, bias);
      if (insertPosition >= 0)
      {
        TextPosition target;
        if (positions[insertPosition].Reference.TryGetTarget(out target))
        {
          return target;
        }
        target = new TextPosition(offset, bias);
        positions[insertPosition] = new HardPosition(target);
        return target;
      }

      var pos = ~insertPosition;
      var retval = new TextPosition(offset, bias);
      positions.Insert(pos, new HardPosition(retval));
      return retval;
    }

    public void InsertAt(int offset, int length)
    {
      PruneObsolete();

      for (var i = positions.Count - 1; i >= 0; i--)
      {
        var pos = positions[i];
        if (pos.Offset < offset)
        {
          // the list is sorted, so once we hit the first element that is smaller 
          // than the offset we are looking for, we can stop iterating.
          break;
        }
        if (pos.Offset == offset && pos.Bias == Bias.Forward)
        {
          // dont modify the start of a element or selection.
          break;
        }

        TextPosition target;
        if (!pos.Reference.TryGetTarget(out target))
        {
          positions.RemoveAt(i);
        }
        else
        {
          target.Offset += length;
          positions[i] = new HardPosition(target);
        }
      }

      positions.Sort();
    }

    public void RemoveAt(int offset, int length)
    {
      PruneObsolete();
      var endOffset = offset + length;
      for (var i = positions.Count - 1; i >= 0; i--)
      {
        var pos = positions[i];
        if (pos.Offset < offset)
        {
          // the list is sorted, so once we hit the first element that is smaller 
          // than the offset we are looking for, we can stop iterating.
          break;
        }
        TextPosition target;
        if (!pos.Reference.TryGetTarget(out target))
        {
          positions.RemoveAt(i);
        }
        else
        {
          if (target.Offset >= endOffset)
          {
            target.Offset -= length;
            positions[i] = new HardPosition(target);
          }
          else if (target.Offset > offset)
          {
            target.Offset = offset;
            positions[i] = new HardPosition(target);
          }
        }
      }

      positions.Sort();
    }

    int FindInsertPosition(int offset, Bias bias)
    {
      return positions.BinarySearch(new HardPosition(offset, bias));
    }

    bool IsReferenceDead(HardPosition position)
    {
      TextPosition target;
      return !position.Reference.TryGetTarget(out target);
    }

    void PruneObsolete()
    {
      positions.RemoveAll(IsReferenceDead);
    }

    struct HardPosition : IComparable<HardPosition>
    {
      public Bias Bias { get; }

      public int Offset { get; }

      public WeakReference<TextPosition> Reference { get; }

      public HardPosition(TextPosition reference)
      {
        Reference = new WeakReference<TextPosition>(reference);
        Offset = reference.Offset;
        Bias = reference.Bias;
      }

      public HardPosition(int offset, Bias bias)
      {
        Bias = bias;
        Reference = null;
        Offset = offset;
      }

      public int CompareTo(HardPosition other)
      {
        var c1 = Offset.CompareTo(other.Offset);
        if (c1 != 0)
        {
          return c1;
        }
        var b1 = (int)Bias;
        var b2 = (int)other.Bias;
        return b1.CompareTo(b2);
      }

      public override string ToString()
      {
        return $"HardTextPosition={{Offset: {Offset}, Bias: {Bias}, Alive: {Reference}}}";
      }
    }

    class TextPosition : ITextPosition
    {
      public TextPosition(int offset, Bias bias)
      {
        Offset = offset;
        Bias = bias;
      }

      public Bias Bias { get; }

      public int Offset { get; set; }

      public override string ToString()
      {
        return $"TextPosition={{Offset: {Offset}, Bias: {Bias}}}";
      }
    }
  }
}