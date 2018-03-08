using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.MemberDeclarationSyntaxExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class MemberDeclarationSyntaxExtensionsTests
	{
		[Test]
		public static void ExamineWithNullThis() =>
			Assert.That(() => (null as MemberDeclarationSyntax).Examine(new ExpressionBodiedData()),
				Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void ExamineWithNullData()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public static class Foo { public static void Bar() { } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			Assert.That(() => syntax.Examine(null),
				Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingAbstractMember()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public abstract class Foo { public abstract int Bar(int x); }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingArrowSingleLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public static class Foo { public static int Bar() => 1; }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(1), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingArrowMultiLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit($"public static class Foo {{ public static int Bar() => 1 + {Environment.NewLine} 2; }}")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(1), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBlockWithMultipleStatements()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { x = x + 2; return x * 3; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBlockSingleLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { return x * 3; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(1), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBlockMultiLine()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit($"public class Foo {{ public int Bar(int x) {{ return x * {Environment.NewLine} 3; }} }}")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(1), nameof(data.BlockMultiLineOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberWithDiagnostics()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar() => 1 }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new ExpressionBodiedData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockSingleLineOccurences, Is.EqualTo(0), nameof(data.BlockSingleLineOccurences));
			Assert.That(data.BlockMultiLineOccurences, Is.EqualTo(0), nameof(data.BlockMultiLineOccurences));
		}
	}
}
