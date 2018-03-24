using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class SyntaxNodeExtensionsTests
	{
		[Test]
		public static void FindParent()
		{
			var unit = SyntaxFactory.ParseCompilationUnit("public class F { public void Foo() { } }");
			var invocation = unit.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
			var parent = invocation.FindParent<ClassDeclarationSyntax>();

			Assert.That(parent.Identifier.Text, Is.EqualTo("F"));
		}

		[Test]
		public static void FindParentWhenParentDoesNotExist()
		{
			var unit = SyntaxFactory.ParseCompilationUnit("public class F { public void Foo() { } }");
			var invocation = unit.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
			var parent = invocation.FindParent<MethodDeclarationSyntax>();

			Assert.That(parent, Is.Null);
		}
	}
}
