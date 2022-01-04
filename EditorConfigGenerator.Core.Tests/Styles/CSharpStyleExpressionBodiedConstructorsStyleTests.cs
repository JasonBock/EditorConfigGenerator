﻿using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class CSharpStyleExpressionBodiedConstructorsStyleTests
{
	[Test]
	public static void CreateWithCustomSeverity()
	{
		const Severity suggestion = Severity.Suggestion;
		var data = new ExpressionBodiedData();
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(data, suggestion);
		Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
	}

	[Test]
	public static void CreateWithNoData()
	{
		var data = new ExpressionBodiedData();
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
	}

	[Test]
	public static void GetSetting()
	{
		var data = new ExpressionBodiedData(2u, 1u, 1u, 0u);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{CSharpStyleExpressionBodiedConstructorsStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void Add()
	{
		var style1 = new CSharpStyleExpressionBodiedConstructorsStyle(new ExpressionBodiedData(9u, 2u, 3u, 4u));
		var style2 = new CSharpStyleExpressionBodiedConstructorsStyle(new ExpressionBodiedData(90u, 20u, 30u, 40u));
		var style3 = style1.Add(style2);

		var data = style3.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(99u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(22u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(33u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(44u), nameof(data.BlockOccurences));
	}

	[Test]
	public static void AddWithNull()
	{
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(new ExpressionBodiedData());
		Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public static void UpdateWithNull()
	{
		var data = new ExpressionBodiedData(default, default, default, default);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(data);

		Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
	}

	[Test]
	public static void UpdateWithMultipleStatements()
	{
		var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() { var y = 22; this.x = 10 + y; } }", options: Shared.ParseOptions)
			.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(
			new ExpressionBodiedData(default, default, default, default));
		var newStyle = style.Update(ctor);

		var data = newStyle.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
	}

	[Test]
	public static void UpdateWithArrowSingleLine()
	{
		var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() => this.x = 10; }", options: Shared.ParseOptions)
			.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(
			new ExpressionBodiedData(default, default, default, default));
		var newStyle = style.Update(ctor);

		var data = newStyle.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(1u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
	}

	[Test]
	public static void UpdateWithArrowMultiLine()
	{
		var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit($"public class Foo {{ private readonly int x; public Foo() => this.x = {Environment.NewLine} 10; }}", options: Shared.ParseOptions)
			.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(
			new ExpressionBodiedData(default, default, default, default));
		var newStyle = style.Update(ctor);

		var data = newStyle.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(1u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
	}

	[Test]
	public static void UpdateWithBlock()
	{
		var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() { this.x = 10; } }", options: Shared.ParseOptions)
			.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(
			new ExpressionBodiedData(default, default, default, default));
		var newStyle = style.Update(ctor);

		var data = newStyle.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(1u), nameof(data.BlockOccurences));
	}

	[Test]
	public static void UpdateWithDiagonstics()
	{
		var ctor = (ConstructorDeclarationSyntax)SyntaxFactory.ParseCompilationUnit("public class Foo { private readonly int x; public Foo() => this.x = 10 }", options: Shared.ParseOptions)
			.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ConstructorDeclaration);
		var style = new CSharpStyleExpressionBodiedConstructorsStyle(
			new ExpressionBodiedData(default, default, default, default));
		var newStyle = style.Update(ctor);

		var data = newStyle.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
		Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
		Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
		Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
	}
}