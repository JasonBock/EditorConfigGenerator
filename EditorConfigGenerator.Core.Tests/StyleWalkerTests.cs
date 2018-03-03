using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests
{
	[TestFixture]
	public static class StyleWalkerTests
	{
		[Test]
		public static void VisitForCSharpStyleVarForBuiltInTypes()
		{
			var walker = new StyleWalker();
			var compilationUnit = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}

	public static void TypeFoo()
	{
		int a = 1;
	}
}");
			walker.Visit(compilationUnit);

			var data = walker.CSharpStyleVarForBuiltInTypesStyle.Data;

			Assert.That(data.TotalOccurences, Is.EqualTo(2u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}
	}
}
