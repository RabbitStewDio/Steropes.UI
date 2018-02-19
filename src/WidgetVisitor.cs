using System;
using System.Text;
using Steropes.UI.Components;

namespace Steropes.UI
{
  public class WidgetVisitor
  {
    readonly Func<IWidget, bool> visitorFunction;
    readonly Action<IWidget> closeContextFunction;

    public WidgetVisitor(Func<IWidget, bool> visitorFunction, Action<IWidget> closeContextFunction = null)
    {
      this.visitorFunction = visitorFunction ?? throw new ArgumentNullException(nameof(visitorFunction));
      this.closeContextFunction = closeContextFunction;
    }

    public void Visit(IWidget w)
    {
      if (visitorFunction(w))
      {
        w.VisitStructuralChildren(Visit);
      }
      closeContextFunction?.Invoke(w);
    }
    
    static StringBuilder IndentFor(StringBuilder b, int indent, string prefix)
    {
      for (var i = 0; i < prefix.Length; i += 1)
      {
        b.Append(" ");
      }
      for (var i = 0; i < indent; i += 1)
      {
        b.Append("  ");
      }

      return b;
    }

    public static WidgetVisitor PrintLayoutVisitor()
    {
      return PrintLayoutVisitor(Console.WriteLine);
    }

    public static WidgetVisitor PrintLayoutVisitor(Action<string> writeLine)
    {

      int indent = 0;
      return new WidgetVisitor(w =>
                               {
                                 var b = new StringBuilder();
                                 IndentFor(b, indent, "");
                                 b.Append(w.NodeType);
                                 b.Append(" *");
                                 if (w.StyleId != null)
                                 {
                                   b.Append(" Id=").Append(w.StyleId);
                                 }

                                 b.Append(" Classes=").Append(w.StyleClasses).Append(Environment.NewLine);

                                 IndentFor(b, indent, w.NodeType)
                                   .Append(" * Padding={").Append(w.Padding).Append("}").Append(Environment.NewLine);
                                 IndentFor(b, indent, w.NodeType)
                                   .Append(" * Anchor={").Append(w.Anchor).Append("}").Append(Environment.NewLine);
                                 IndentFor(b, indent, w.NodeType)
                                   .Append(" * DesiredSize={").Append(w.DesiredSize).Append("}").Append(Environment.NewLine);
                                 IndentFor(b, indent, w.NodeType)
                                   .Append(" * Layout=");
                                 if (w.LayoutInvalid)
                                 {
                                   b.Append("Invalid");
                                 }
                                 else
                                 {
                                   b.Append(w.LayoutRect);
                                 }

                                 b.Append(Environment.NewLine);

                                 indent += 1;
                                 writeLine(b.ToString());
                                 return true;
                               },
                               w => { indent -= 1; });
    }
  }
}