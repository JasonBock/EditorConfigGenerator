using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class SyntaxNodeExtensionsTests
	{
		[Test]
		public static void ExamineWithNullThis() =>
			Assert.That(() => (null as MemberDeclarationSyntax).Examine(new ExpressionBodiedData()),
				Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void ExamineWithNullData()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public static class Foo { public static void Bar() { } }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			Assert.That(() => syntax.Examine(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingAbstractMember()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public abstract class Foo { public abstract int Bar(int x); }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingArrowSingleLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public static class Foo { public static int Bar() => 1; }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(1), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingArrowMultiLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit($"public static class Foo {{ public static int Bar() => 1 + {Environment.NewLine} 2; }}", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(1), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBlockWithMultipleStatements()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { x = x + 2; return x * 3; } }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBlock()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { return x * 3; } }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(1), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberWithDiagnostics()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar() => 1 }", options: Constants.ParseOptions)
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0), nameof(data.BlockOccurences));
		}

		[Test]
		public static void FindParent()
		{
			var unit = SyntaxFactory.ParseCompilationUnit("public class F { public void Foo() { } }", options: Constants.ParseOptions);
			var invocation = unit.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
			var parent = invocation.FindParent<ClassDeclarationSyntax>();

			Assert.That(parent.Identifier.Text, Is.EqualTo("F"));
		}

		[Test]
		public static void FindParentWhenParentDoesNotExist()
		{
			var unit = SyntaxFactory.ParseCompilationUnit("public class F { public void Foo() { } }", options: Constants.ParseOptions);
			var invocation = unit.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
			var parent = invocation.FindParent<MethodDeclarationSyntax>();

			Assert.That(parent, Is.Null);
		}
	}
}
