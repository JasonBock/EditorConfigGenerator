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
			Assert.That(() => (null as MemberDeclarationSyntax).Examine(new BooleanData()),
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
		public static void ExamineWithExpressionBodiedMemberUsingArrow()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public static class Foo { public static int Bar() => 1; }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new BooleanData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0), nameof(data.FalseOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingAbstractMember()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public abstract class Foo { public abstract int Bar(int x); }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new BooleanData());

			Assert.That(data.TotalOccurences, Is.EqualTo(0), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0), nameof(data.FalseOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBodyWithMultipleStatements()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { x = x + 2; return x * 3; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var current = new BooleanData();
			var data = syntax.Examine(current);

			Assert.That(data, Is.SameAs(current));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberUsingBodyWithOneStatement()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar(int x) { return x * 3; } }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var data = syntax.Examine(new BooleanData());

			Assert.That(data.TotalOccurences, Is.EqualTo(1), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1), nameof(data.FalseOccurences));
		}

		[Test]
		public static void ExamineWithExpressionBodiedMemberWithDiagnostics()
		{
			var syntax = SyntaxFactory.ParseCompilationUnit("public class Foo { public int Bar() => 1 }")
				.DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var current = new BooleanData();
			var data = syntax.Examine(current);

			Assert.That(data, Is.SameAs(current));
		}
	}
}
