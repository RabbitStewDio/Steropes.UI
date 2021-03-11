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
using System.Xml.Linq;

using Steropes.UI.Styles.Conditions;
using Steropes.UI.Styles.Io.Values;

namespace Steropes.UI.Styles.Io.Writer
{
  public class ConditionWriter : IConditionWriter
  {
    readonly AttributeConditionWriter attributeWriter;

    readonly Dictionary<Type, IConditionWriter> writers;

    public ConditionWriter()
    {
      attributeWriter = new AttributeConditionWriter();

      writers = new Dictionary<Type, IConditionWriter>();
      writers.Add(typeof(AttributeCondition), attributeWriter);
      writers.Add(typeof(ClassCondition), new ClassConditionWriter());
      writers.Add(typeof(PseudoClassCondition), new PseudoClassConditionWriter());
      writers.Add(typeof(IdCondition), new IdConditionWriter());
      writers.Add(typeof(NotCondition), new NotConditionWriter());
      writers.Add(typeof(AndCondition), new AndConditionWriter());
      writers.Add(typeof(OrCondition), new OrConditionWriter());
    }

    public void Register(IStylePropertySerializer p)
    {
      attributeWriter.Register(p);
    }

    public void Write(IStyleSystem styleSystem, XContainer container, ICondition condition, IConditionWriter childWriter)
    {
      IConditionWriter w;
      if (writers.TryGetValue(condition.GetType(), out w))
      {
        w.Write(styleSystem, container, condition, childWriter);
      }
      else
      {
        throw new StyleWriterException("There is no writer for condition " + condition.GetType().Name);
      }
    }
  }
}