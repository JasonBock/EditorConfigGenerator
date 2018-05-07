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
	public static class CSharpStyleExpressionBodiedOperatorsStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new ExpressionBodiedData();
			var style = new CSharpStyleExpressionBodiedOperatorsStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new ExpressionBodiedData();
			var style = new CSharpStyleExpressionBodiedOperatorsStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void GetSetting()
		{
			var data = new ExpressionBodiedData(2u, 1u, 1u, 0u);
			var style = new CSharpStyleExpressionBodiedOperatorsStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_style_expression_bodied_operators = true:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpStyleExpressionBodiedOperatorsStyle(new ExpressionBodiedData(1u, 2u, 3u, 4u));
			var style2 = new CSharpStyleExpressionBodiedOperatorsStyle(new ExpressionBodiedData(10u, 20u, 30u, 40u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(22u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(33u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(44u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpStyleExpressionBodiedOperatorsStyle(new ExpressionBodiedData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new ExpressionBodiedData(default, default, default, default);
			var style = new CSharpStyleExpressionBodiedOperatorsStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithMultipleStatements()
		{
			var @operator = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) 
	{ 
		var d1 = f1.Data; 
		var d2 = f2.Data; 
		return d1 + d2; 
	} 

	public int Data { get; } 
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.OperatorDeclaration) as OperatorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedOperatorsStyle(
				new ExpressionBodiedData(default, default, default, default));
			var newStyle = style.Update(@operator);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithArrowSingleLine()
		{
			var @operator = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) => f1.Data + f2.Data;

	public int Data { get; } 
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.OperatorDeclaration) as OperatorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedOperatorsStyle(
				new ExpressionBodiedData(default, default, default, default));
			var newStyle = style.Update(@operator);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(1u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithArrowMultiLine()
		{
			var @operator = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) => f1.Data + 
		f2.Data;

	public int Data { get; } 
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.OperatorDeclaration) as OperatorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedOperatorsStyle(
				new ExpressionBodiedData(default, default, default, default));
			var newStyle = style.Update(@operator);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(1u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithBlock()
		{
			var @operator = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) { return f1.Data + f2.Data; }

	public int Data { get; } 
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.OperatorDeclaration) as OperatorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedOperatorsStyle(
				new ExpressionBodiedData(default, default, default, default));
			var newStyle = style.Update(@operator);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(1u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithDiagonstics()
		{
			var @operator = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) => f1.Data + f2.Data

	public int Data { get; } 
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.OperatorDeclaration) as OperatorDeclarationSyntax;

			var style = new CSharpStyleExpressionBodiedOperatorsStyle(
				new ExpressionBodiedData(default, default, default, default));
			var newStyle = style.Update(@operator);

			var data = newStyle.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}
	}
}
