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
using System.Collections.Generic;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Edits
{
  public class CompoundUndoableEdit : UndoableEditBase
  {
    readonly List<IUndoableEdit> edits;

    public CompoundUndoableEdit() : base(true, "")
    {
      edits = new List<IUndoableEdit>();
    }

    public override string DisplayName
    {
      get
      {
        if (edits.Count == 0)
        {
          return "";
        }
        if (edits.Count == 1 || IsUndo)
        {
          return edits[0].DisplayName;
        }

        var extras = edits.Count - 1;
        return $"{edits[extras].DisplayName} and {extras} more actions";
      }
    }

    public override bool Add(IUndoableEdit other)
    {
      if (other == null)
      {
        return false;
      }

      var compoundUndoableEdit = other as CompoundUndoableEdit;
      if (compoundUndoableEdit != null)
      {
        var co = compoundUndoableEdit;
        for (var index = 0; index < co.edits.Count; index++)
        {
          var edit = co.edits[index];
          edits.Add(edit);
          AddNotify(edit);
        }
      }
      else
      {
        edits.Add(other);
        AddNotify(other);
      }
      return true;
    }

    protected virtual void AddNotify(IUndoableEdit edit)
    {
    }

    protected virtual void BeginRedo()
    {
    }

    protected virtual void BeginUndo()
    {
    }

    protected virtual void EndRedo()
    {
    }

    protected virtual void EndUndo()
    {
    }

    protected override void PerformRedo()
    {
      BeginRedo();
      for (var i = 0; i < edits.Count; i++)
      {
        var edit = edits[i];
        edit.Redo();
      }
      EndRedo();
    }

    protected override void PerformUndo()
    {
      BeginUndo();
      for (var i = edits.Count - 1; i >= 0; i--)
      {
        var edit = edits[i];
        edit.Undo();
      }
      EndUndo();
    }
  }
}