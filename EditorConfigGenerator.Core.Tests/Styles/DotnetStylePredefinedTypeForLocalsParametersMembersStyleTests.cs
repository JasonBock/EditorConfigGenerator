using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	[Parallelizable(ParallelScope.Self)]
	public static class DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("dotnet_style_predefined_type_for_locals_parameters_members = false:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("dotnet_style_predefined_type_for_locals_parameters_members = true:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		private static (T, SemanticModel) GetInformation<T>(CompilationUnitSyntax unit)
			where T : SyntaxNode
		{
			var node = unit.DescendantNodes().OfType<T>().First();
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (node, model);
		}

		[Test]
		public static void UpdateWithParameterThatIsPredefinedTypeUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar(int x) { }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<ParameterSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithParameterThatIsPredefinedTypeNotUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo 
{ 
	public void Bar(Int32 x) { }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<ParameterSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithParameterThatIsNotPredefinedType()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar(Foo x) { }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<ParameterSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithLocalDeclarationThatIsPredefinedTypeUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar() 
	{ 
		int x = 10;
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<LocalDeclarationStatementSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithLocalDeclarationThatIsPredefinedTypeNotUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo 
{ 
	public void Bar() 
	{ 
		Int32 x = 10;
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<LocalDeclarationStatementSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithLocalDeclarationThatIsNotPredefinedType()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar() 
	{ 
		Foo x = default;
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<LocalDeclarationStatementSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithFieldDeclarationThatIsPredefinedTypeUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	private int x;
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<FieldDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithFieldDeclarationThatIsPredefinedTypeNotUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo 
{ 
	private Int32 x;
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<FieldDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithFieldDeclarationThatIsNotPredefinedType()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	private Foo x;
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<FieldDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithPropertyDeclarationThatIsPredefinedTypeUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public int X { get; }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<PropertyDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithPropertyDeclarationThatIsPredefinedTypeNotUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo 
{ 
	public Int32 X { get; }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<PropertyDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithPropertyDeclarationThatIsNotPredefinedType()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public Foo X { get; }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<PropertyDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithUnexpectedSyntaxNodeType()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public Foo X { get; }
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<ClassDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		int x =>;
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForLocalsParametersMembersStyleTests.GetInformation<LocalDeclarationStatementSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
