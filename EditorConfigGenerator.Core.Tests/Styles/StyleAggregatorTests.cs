using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class StyleAggregatorTests
	{
		private static SemanticModel CreateModel(SyntaxTree tree)
		{
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			return compilation.GetSemanticModel(tree);
		}

		[Test]
		public static void Add()
		{
			var aggregator1 = new StyleAggregator();
			var compilationUnit1 = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}", options: Shared.ParseOptions);
			aggregator1 = aggregator1.Add(compilationUnit1, StyleAggregatorTests.CreateModel(compilationUnit1.SyntaxTree));

			var aggregator2 = new StyleAggregator();
			var compilationUnit2 = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void TypeFoo()
	{
		int a = 1;
	}
}", options: Shared.ParseOptions);
			aggregator2 = aggregator2.Add(compilationUnit2, StyleAggregatorTests.CreateModel(compilationUnit2.SyntaxTree));

			var aggregator3 = aggregator1.Update(aggregator2);
			var data3 = aggregator3.Set.CSharpStyleVarForBuiltInTypesStyle.Data;

			Assert.That(data3.TotalOccurences, Is.EqualTo(2u), nameof(data3.TotalOccurences));
			Assert.That(data3.TrueOccurences, Is.EqualTo(1u), nameof(data3.TrueOccurences));
			Assert.That(data3.FalseOccurences, Is.EqualTo(1u), nameof(data3.FalseOccurences));
		}

		[Test]
		public static void AddWithNullSyntax()
		{
			var compilationUnit = SyntaxFactory.ParseCompilationUnit("public static class Test { }", options: Shared.ParseOptions);
			var model = StyleAggregatorTests.CreateModel(compilationUnit.SyntaxTree);
			var aggregator = new StyleAggregator();
			Assert.That(() => aggregator.Add(null, model), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void AddWithNullModel()
		{
			var compilationUnit = SyntaxFactory.ParseCompilationUnit("public static class Test { }", options: Shared.ParseOptions);
			var aggregator = new StyleAggregator();
			Assert.That(() => aggregator.Add(compilationUnit, null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void VisitForCSharpStyleVarForBuiltInTypesStyle()
		{
			var aggregator = new StyleAggregator();
			var style = aggregator.Set.CSharpStyleVarForBuiltInTypesStyle;
			var compilationUnit = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}", options: Shared.ParseOptions);
			aggregator = aggregator.Add(compilationUnit, StyleAggregatorTests.CreateModel(compilationUnit.SyntaxTree));

			Assert.That(aggregator.Set.CSharpStyleVarForBuiltInTypesStyle, Is.Not.SameAs(style));
		}

		[Test]
		public static void VisitForCSharpNewLineBeforeCatchStyle()
		{
			var aggregator = new StyleAggregator();
			var style = aggregator.Set.CSharpNewLineBeforeCatchStyle;
			var compilationUnit = SyntaxFactory.ParseCompilationUnit(
@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}", options: Shared.ParseOptions);
			aggregator = aggregator.Add(compilationUnit, StyleAggregatorTests.CreateModel(compilationUnit.SyntaxTree));
			var data = aggregator.Set.CSharpStyleVarForBuiltInTypesStyle.Data;

			Assert.That(aggregator.Set.CSharpStyleVarForBuiltInTypesStyle, Is.Not.SameAs(style));
		}
	}
}
