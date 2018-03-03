using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests
{
	[TestFixture]
	public static class StyleWalkerTests
	{
		[Test]
		public static void Add()
		{
			var walker1 = new StyleWalker();
			var compilationUnit1 = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}");
			walker1.Visit(compilationUnit1);

			var walker2 = new StyleWalker();
			var compilationUnit2 = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void TypeFoo()
	{
		int a = 1;
	}
}");
			walker2.Visit(compilationUnit2);

			var walker3 = walker1.Add(walker2);
			var data3 = walker3.CSharpStyleVarForBuiltInTypesStyle.Data;

			Assert.That(data3.TotalOccurences, Is.EqualTo(2u), nameof(data3.TotalOccurences));
			Assert.That(data3.TrueOccurences, Is.EqualTo(1u), nameof(data3.TrueOccurences));
			Assert.That(data3.FalseOccurences, Is.EqualTo(1u), nameof(data3.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var walker = new StyleWalker();
			Assert.That(() => walker.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

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
