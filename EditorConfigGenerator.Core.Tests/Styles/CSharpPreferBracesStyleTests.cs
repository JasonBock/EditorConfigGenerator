using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class CSharpPreferBracesStyleTests
{
   [Test]
   public static void CreateWithCustomSeverity()
   {
	  const Severity suggestion = Severity.Suggestion;
	  var data = new BooleanData();
	  var style = new CSharpPreferBracesStyle(data, suggestion);
	  Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
   }

   [Test]
   public static void CreateWithNoData()
   {
	  var data = new BooleanData();
	  var style = new CSharpPreferBracesStyle(data);
	  Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
	  Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
   }

   [Test]
   public static void CreateWithMoreFalseData()
   {
	  var data = new BooleanData(1u, 0u, 1u);
	  var style = new CSharpPreferBracesStyle(data);
	  Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
	  Assert.That(style.GetSetting(), Is.EqualTo(
		  $"{CSharpPreferBracesStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
   }

   [Test]
   public static void CreateWithMoreTrueData()
   {
	  var data = new BooleanData(1u, 1u, 0u);
	  var style = new CSharpPreferBracesStyle(data);
	  Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
	  Assert.That(style.GetSetting(), Is.EqualTo(
		  $"{CSharpPreferBracesStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
   }

   [Test]
   public static void Add()
   {
	  var style1 = new CSharpPreferBracesStyle(new BooleanData(3u, 1u, 2u));
	  var style2 = new CSharpPreferBracesStyle(new BooleanData(30u, 10u, 20u));
	  var style3 = style1.Add(style2);

	  var data = style3.Data;
	  Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void AddWithNull()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData());
	  Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
   }

   [Test]
   public static void UpdateWithNull()
   {
	  var data = new BooleanData(default, default, default);
	  var style = new CSharpPreferBracesStyle(data);

	  Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
   }

   [Test]
   public static void UpdateWithUnexpectedNode()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ClassDeclarationSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo { }", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ClassDeclaration);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithIfStatementMultiline()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (IfStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
		{
			var x = 2;
			var y = x;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithIfStatementOneLineWithBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (IfStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithIfStatementOneLineWithoutBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (IfStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true)
			var x = 2;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithElseClauseMultiline()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ElseClauseSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true) { }
		else
		{
			var x = 2;
			var y = x;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithElseClauseOneLineWithBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ElseClauseSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true) { }
		else
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithElseClauseOneLineWithoutBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ElseClauseSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true) { }
		else
			var x = 2;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ElseClause);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForStatementMultiline()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		for (var i = 0; i < 10; i++)
		{
			var x = 2;
			var y = x;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForStatementOneLineWithBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		for (var i = 0; i < 10; i++)
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForStatementOneLineWithoutBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		for (var i = 0; i < 10; i++)
			var x = 2;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForEachStatementMultiline()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForEachStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		foreach (var item in items)
		{
			var x = 2;
			var y = x;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForEachStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForEachStatementOneLineWithBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForEachStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		foreach (var item in items)
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForEachStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithForEachStatementOneLineWithoutBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (ForEachStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		foreach (var item in items)
			var x = 2;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForEachStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithWhileStatementMultiline()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (WhileStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		while (true)
		{
			var x = 2;
			var y = x;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.WhileStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithWhileStatementOneLineWithBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (WhileStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		while (true)
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.WhileStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }

   [Test]
   public static void UpdateWithWhileStatementOneLineWithoutBlock()
   {
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (WhileStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar(string[] items)
	{
		while (true)
			var x = 2;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.WhileStatement);
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
	  var style = new CSharpPreferBracesStyle(new BooleanData(default, default, default));

	  var statement = (IfStatementSyntax)SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if (true)=>
		{
			var x = 2;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement);
	  var newStyle = style.Update(statement);

	  var data = newStyle.Data;
	  Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
	  Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
	  Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
	  Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
   }
}