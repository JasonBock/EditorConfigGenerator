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
	public static class CSharpSpaceBetweenParenthesesStyleTests
	{
		[Test]
		public static void CreateWithNoData()
		{
			var data = new ParenthesesSpaceData();
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreControlFlowFalseData()
		{
			var data = new ParenthesesSpaceData(1u, 1u, 0u, 0u, 0u, 0u, 0u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreControlFlowTrueData()
		{
			var data = new ParenthesesSpaceData(1u, 0u, 1u, 0u, 0u, 0u, 0u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_space_between_parentheses = control_flow_statements"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreExpressionsFalseData()
		{
			var data = new ParenthesesSpaceData(1u, 0u, 0u, 1u, 0u, 0u, 0u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreExpressionsTrueData()
		{
			var data = new ParenthesesSpaceData(1u, 0u, 0u, 0u, 1u, 0u, 0u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_space_between_parentheses = expressions"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTypeCastsFalseData()
		{
			var data = new ParenthesesSpaceData(1u, 0u, 0u, 0u, 0u, 1u, 0u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTypeCastsTrueData()
		{
			var data = new ParenthesesSpaceData(1u, 0u, 0u, 0u, 0u, 0u, 1u);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_space_between_parentheses = type_casts"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpSpaceBetweenParenthesesStyle(new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u));
			var style2 = new CSharpSpaceBetweenParenthesesStyle(new ParenthesesSpaceData(210u, 10u, 20u, 30u, 40u, 50u, 60u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(231u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(11u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(22u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(33u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(44u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(55u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(66u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(new ParenthesesSpaceData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new ParenthesesSpaceData(default, default, default, default, default, default, default);
			var style = new CSharpSpaceBetweenParenthesesStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithUnexpectedSyntaxNode()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit("public class Foo { }", options: Shared.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ClassDeclaration) as ClassDeclarationSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithForStatementWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		for(var i = 0; i < 10; i++) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForStatement) as ForStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithForStatementWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		for( var i = 0; i < 10; i++ ) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForStatement) as ForStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithForEachStatementWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var data = new int[] { 1, 2, 3 };
		foreach(var item in data) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForEachStatement) as ForEachStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithForEachStatementWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var data = new int[] { 1, 2, 3 };
		foreach( var item in data ) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ForEachStatement) as ForEachStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithIfStatementWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if(true) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement) as IfStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithIfStatementWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		if( true ) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.IfStatement) as IfStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithSwitchStatementWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = true;
		
		switch(x)
		{
			case true:
				break;
			case false:
				break;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.SwitchStatement) as SwitchStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithSwitchStatementWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = true;
		
		switch( x )
		{
			case true:
				break;
			case false:
				break;
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.SwitchStatement) as SwitchStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithWhileStatementWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		while(true) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.WhileStatement) as WhileStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithWhileStatementWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		while( true ) { }
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.WhileStatement) as WhileStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithParenthesizedExpressionWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = (3 + 2);
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ParenthesizedExpression) as ParenthesizedExpressionSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithParenthesizedExpressionWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = ( 3 + 2 );
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ParenthesizedExpression) as ParenthesizedExpressionSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(1u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithCastExpressionWithNoSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = (uint)22;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.CastExpression) as CastExpressionSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(1u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithCastExpressionWithSpaces()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = ( uint )22;
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.CastExpression) as CastExpressionSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(1u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new CSharpSpaceBetweenParenthesesStyle(
				new ParenthesesSpaceData(default, default, default, default, default, default, default));

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		try
		{
		} 
		finally
		}
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.FinallyClause) as FinallyClauseSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(0u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(0u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(0u), nameof(data.TypeCastsSpaceOccurences));
		}
	}
}
