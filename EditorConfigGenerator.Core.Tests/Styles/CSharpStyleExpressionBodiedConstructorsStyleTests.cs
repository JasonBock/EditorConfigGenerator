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
	public static class CSharpStyleExpressionBodiedConstructorsStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_style_expression_bodied_constructors = false:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_style_expression_bodied_constructors = true:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithExpressionBody()
		{
			var ctorDeclaration = SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() => this.x = 10; }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration) as ConstructorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(default, default, default));
			var newStyle = style.Update(ctorDeclaration);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithMultipleStatements()
		{
			var ctorDeclaration = SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() { var y = 22; this.x = 10 + y; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration) as ConstructorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(default, default, default));
			var newStyle = style.Update(ctorDeclaration);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithOneStatement()
		{
			var ctorDeclaration = SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() { this.x = 10; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration) as ConstructorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(default, default, default));
			var newStyle = style.Update(ctorDeclaration);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagonstics()
		{
			var ctorDeclaration = SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() => this.x = 10 }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration) as ConstructorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedConstructorsStyle(new BooleanData(default, default, default));
			var newStyle = style.Update(ctorDeclaration);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
