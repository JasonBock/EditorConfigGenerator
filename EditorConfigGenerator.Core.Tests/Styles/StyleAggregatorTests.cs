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

		private static void TestStyleVisitation(string code, string stylePropertyName)
		{
			var aggregator = new StyleAggregator();
			var setProperty = aggregator.GetType().GetProperty(nameof(StyleAggregator.Set));
			var styleProperty = setProperty.PropertyType.GetProperty(stylePropertyName);
			var styleDataProperty = styleProperty.PropertyType.GetProperty("Data");
			var data = styleDataProperty.GetValue(styleProperty.GetValue(setProperty.GetValue(aggregator)));

			var compilationUnit = SyntaxFactory.ParseCompilationUnit(code, options: Shared.ParseOptions);

			aggregator = aggregator.Add(compilationUnit, StyleAggregatorTests.CreateModel(compilationUnit.SyntaxTree));
			var newData = styleDataProperty.GetValue(styleProperty.GetValue(setProperty.GetValue(aggregator)));

			Assert.That(newData, Is.Not.EqualTo(data));
		}

		[Test]
		public static void VisitForCSharpNewLineBeforeCatchStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static void Foo()
	{
		try { }
		catch { }
	}
}", nameof(IStyleSet.CSharpNewLineBeforeCatchStyle));

		[Test]
		public static void VisitForCSharpNewLineBeforeElseStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static void Foo()
	{
		if(true) { }
		else { }
	}
}", nameof(IStyleSet.CSharpNewLineBeforeElseStyle));

		[Test]
		public static void VisitForCSharpNewLineBeforeFinallyStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static void Foo()
	{
		try { }
		finally { }
	}
}", nameof(IStyleSet.CSharpNewLineBeforeFinallyStyle));

		[Test]
		public static void VisitForCSharpNewLineBeforeMembersInAnonymousTypesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static void Foo()
	{
		var x = new { A = 1, B = 2 };
	}
}", nameof(IStyleSet.CSharpNewLineBeforeMembersInAnonymousTypesStyle));

		[Test]
		public static void VisitForCSharpNewLineBeforeMembersInObjectInitializersStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Data { public int A; public int B; }

public class Foo
{
	public void Bar()
	{
		var x = new Data 
		{ 
			A = 1, 
			B = 2
		}
	}
}", nameof(IStyleSet.CSharpNewLineBeforeMembersInObjectInitializersStyle));

		[Test]
		public static void VisitForCSharpNewLineBetweenQueryExpressionClausesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		var x = new int[] { 1, 2, 3 };
		var q = from a in x
				  from b in x
				  select a * b;
	}
}", nameof(IStyleSet.CSharpNewLineBetweenQueryExpressionClausesStyle));

		[Test]
		public static void VisitForCSharpPreferBracesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		if(true)
			var x = 2;
	}
}", nameof(IStyleSet.CSharpPreferBracesStyle));

		[Test]
		public static void VisitForCSharpPreferredModifierOrderStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public static void Bar() { }
}", nameof(IStyleSet.CSharpPreferredModifierOrderStyle));

		[Test]
		public static void VisitCSharpPreferSimpleDefaultExpressionStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		string x = default(string);
	}
}", nameof(IStyleSet.CSharpPreferSimpleDefaultExpressionStyle));

		[Test]
		public static void VisitCSharpPreserveSingleLineBlocksStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public int X { get; set; }
}", nameof(IStyleSet.CSharpPreserveSingleLineBlocksStyle));

		[Test]
		public static void VisitCSharpSpaceAfterCastStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar(object x)
	{
		var y = (int) x;
	}
}", nameof(IStyleSet.CSharpSpaceAfterCastStyle));

		[Test]
		public static void VisitCSharpSpaceAfterKeywordsInControlFlowStatementsStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar(object x)
	{
		for (var i = 0; i < 10; i++) { }
	}
}", nameof(IStyleSet.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle));

		[Test]
		public static void VisitCSharpSpaceBetweenMethodCallParameterListParenthesesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar(int x) 
	{
		Bar( 3 );
	}
}", nameof(IStyleSet.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle));

		[Test]
		public static void VisitCSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar( object x ) { }
}", nameof(IStyleSet.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle));

		[Test]
		public static void VisitCSharpSpaceBetweenParenthesesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		for(var i = 0; i < 10; i++) { }
	}
}", nameof(IStyleSet.CSharpSpaceBetweenParenthesesStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedAccessorsStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	private int age; 

	public int Age 
	{
		get => age; 
	} 
}", nameof(IStyleSet.CSharpStyleExpressionBodiedAccessorsStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedConstructorsStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	private readonly int x; 
	
	public Foo() => this.x = 10;
}", nameof(IStyleSet.CSharpStyleExpressionBodiedConstructorsStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedIndexersStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	private int[] ages; 

	public int this[int i] => this.ages[i];
}", nameof(IStyleSet.CSharpStyleExpressionBodiedIndexersStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedMethodsStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	public int Foo() 
	{ 
		return 10; 
	} 
}", nameof(IStyleSet.CSharpStyleExpressionBodiedMethodsStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedOperatorsStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	public static Foo operator +(Foo f1, Foo f2) => f1.Data + 
		f2.Data;

	public int Data { get; } 
}", nameof(IStyleSet.CSharpStyleExpressionBodiedOperatorsStyle));

		[Test]
		public static void VisitCSharpStyleExpressionBodiedPropertiesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	private int age; 

	public int Age => age;
}", nameof(IStyleSet.CSharpStyleExpressionBodiedPropertiesStyle));

		[Test]
		public static void VisitCSharpStyleInlinedVariableDeclarationStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{
	public void Foo(out a) { Foo(out int x); }
}", nameof(IStyleSet.CSharpStyleInlinedVariableDeclarationStyle));

		[Test]
		public static void VisitCSharpStylePatternLocalOverAnonymousFunctionStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		int X() => 22;
	}
}", nameof(IStyleSet.CSharpStylePatternLocalOverAnonymousFunctionStyle));

		[Test]
		public static void VisitCSharpStylePatternMatchingOverAsWithNullCheckStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar(object o)
	{
		if(o is string s) { }
	}
}", nameof(IStyleSet.CSharpStylePatternMatchingOverAsWithNullCheckStyle));

		[Test]
		public static void VisitForCSharpStyleVarElsewhereStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static int Foo()
	{
		var x = Foo();
	}
}", nameof(IStyleSet.CSharpStyleVarElsewhereStyle));

		[Test]
		public static void VisitForCSharpStyleVarForBuiltInTypesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public static class Test
{
	public static void Foo()
	{
		var a = 1;
	}
}", nameof(IStyleSet.CSharpStyleVarForBuiltInTypesStyle));

		[Test]
		public static void VisitCSharpStyleVarWhenTypeIsApparentStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Customer { } 

public static class Test
{
	public static void Foo()
	{
		var customer = new Customer();
	}
}", nameof(IStyleSet.CSharpStyleVarWhenTypeIsApparentStyle));

		[Test]
		public static void VisitDotnetSortSystemDirectivesFirstStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"using System;
using System.String;
public class Foo { }", nameof(IStyleSet.DotnetSortSystemDirectivesFirstStyle));

		[Test]
		public static void VisitDotnetStyleExplicitTupleNamesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	private int id;

	public void Bar()
	{
		(string name, int id) data = (""x"", 42);
		var x = data.Item1;
	}
}", nameof(IStyleSet.DotnetStyleExplicitTupleNamesStyle));

		[Test]
		public static void VisitDotnetStyleObjectInitializerStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data { Value = 22 };
	}
}", nameof(IStyleSet.DotnetStyleObjectInitializerStyle));

		[Test]
		public static void VisitDotnetStylePredefinedTypeForLocalsParametersMembersStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	public void Bar(int x) { }
}", nameof(IStyleSet.DotnetStylePredefinedTypeForLocalsParametersMembersStyle));

		[Test]
		public static void VisitDotnetStylePredefinedTypeForMemberAccessStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	public int Bar() => int.MaxValue;
}", nameof(IStyleSet.DotnetStylePredefinedTypeForMemberAccessStyle));

		[Test]
		public static void VisitDotnetStylePreferInferredAnonymousTypeMemberNamesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		var age = 42;
		var anon = new { age };
	}
}", nameof(IStyleSet.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle));

		[Test]
		public static void VisitDotnetStylePreferInferredTupleNamesStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public void Bar()
	{
		var age = 42;
		var name = ""Jane"";
		var tuple = (age, name);
	}
}", nameof(IStyleSet.DotnetStylePreferInferredTupleNamesStyle));

		[Test]
		public static void VisitDotnetStyleQualificationForEventStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public event EventHandler DoIt;

	public void Bar()
	{
		this.DoIt += (a, b) => { };
	}
}", nameof(IStyleSet.DotnetStyleQualificationForEventStyle));

		[Test]
		public static void VisitDotnetStyleQualificationForFieldStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	private int x;

	public int Bar() => this.x;
}", nameof(IStyleSet.DotnetStyleQualificationForFieldStyle));

		[Test]
		public static void VisitDotnetStyleQualificationForMethodStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo 
{ 
	public void Bar()
	{
		this.Bar();
	}
}", nameof(IStyleSet.DotnetStyleQualificationForMethodStyle));

		[Test]
		public static void VisitDotnetStyleQualificationForPropertyStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"public class Foo
{
	public int X { get; set; }

	public int Bar() => this.X;
}", nameof(IStyleSet.DotnetStyleQualificationForPropertyStyle));

		[Test]
		public static void VisitDotnetStyleRequireAccessibilityModifiersStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"class Foo
{
	private void Bar() { }
}", nameof(IStyleSet.DotnetStyleRequireAccessibilityModifiersStyle));

		[Test]
		public static void VisitIndentStyleStyle() =>
			StyleAggregatorTests.TestStyleVisitation(
@"class Foo
{
	private void Bar() { }
}", nameof(IStyleSet.IndentStyleStyle));
	}
}
