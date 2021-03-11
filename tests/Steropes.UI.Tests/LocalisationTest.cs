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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

using FluentAssertions;

using NUnit.Framework;

using Steropes.UI.I18N;

namespace Steropes.UI.Test
{
  [Category("Platform Behaviour")]
  public class LocalisationTest
  {
    // https://msdn.microsoft.com/en-us/library/cc233965.aspx
    // [MS-LCID]: Windows Language Code Identifier (LCID) Reference
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "WinAPI")]

    // ReSharper disable once InconsistentNaming
    const int LCID_FRENCH = 0x040c;

    [Test]
    public void CommonLocalisations_En()
    {
      Common.Yes.Should().Be("Yes");
    }

    [Test]
    public void CommonLocalisations_Fr()
    {
      var culture = Thread.CurrentThread.CurrentUICulture;
      try
      {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(LCID_FRENCH);
        Common.Yes.Should().Be("Oui");
      }
      finally
      {
        Thread.CurrentThread.CurrentUICulture = culture;
      }
    }
  }
}