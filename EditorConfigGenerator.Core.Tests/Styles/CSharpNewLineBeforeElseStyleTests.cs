using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class CSharpNewLineBeforeElseStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new CSharpNewLineBeforeElseStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new CSharpNewLineBeforeElseStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new CSharpNewLineBeforeElseStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_new_line_before_else = false:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new CSharpNewLineBeforeElseStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_new_line_before_else = true:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpNewLineBeforeElseStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new CSharpNewLineBeforeElseStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new CSharpNewLineBeforeElseStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithLineBeforeElse()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
		{
		}
		else
		{
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause) as ElseClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithNoLineBeforeElse()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
		{
		} else
		{
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause) as ElseClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithLineBeforeElseAndNoBlock()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
			var x = 42;
		else
		{
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause) as ElseClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithNoLineBeforeElseAndNoBlock()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
			var x = 42; else
		{
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause) as ElseClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new CSharpNewLineBeforeElseStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
		{
		}
		else =>
		{
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause) as ElseClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
