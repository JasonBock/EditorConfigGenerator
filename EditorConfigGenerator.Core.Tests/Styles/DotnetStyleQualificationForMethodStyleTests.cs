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
	public static class DotnetStyleQualificationForMethodStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new DotnetStyleQualificationForMethodStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new DotnetStyleQualificationForMethodStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new DotnetStyleQualificationForMethodStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("dotnet_style_qualification_for_method = false:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new DotnetStyleQualificationForMethodStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("dotnet_style_qualification_for_method = true:error"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new DotnetStyleQualificationForMethodStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new DotnetStyleQualificationForMethodStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new DotnetStyleQualificationForMethodStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		private static (InvocationExpressionSyntax, SemanticModel) GetInformation(
			CompilationUnitSyntax unit, string methodName)
		{
			var invocation = unit.DescendantNodes().OfType<InvocationExpressionSyntax>()
				.First(_ => _.ToString().Contains(methodName));
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (invocation, model);
		}

		[Test]
		public static void UpdateWithInstanceMethodQualifiedWithThis()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar()
	{
		this.Bar();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "Bar");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithInstanceMethodNotQualifiedWithThis()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar()
	{
		Bar();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "Bar");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithInstanceMethodQualifiedWithVariableName()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar()
	{
		var x = 42;
		x.ToString();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "ToString");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithExtensionMethod()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{
	public static void CallFoo(this Foo @this) { }

	public void Bar()
	{
		this.CallFoo();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "CallFoo");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithStaticMethodWithClassName()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static void CallMe() { }

	public void Bar()
	{
		Foo.CallMe();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "CallMe");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithStaticMethodWithoutClassName()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public static void CallMe() { }

	public void Bar()
	{
		CallMe();
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "CallMe");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new DotnetStyleQualificationForMethodStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public void Bar()
	{
		this.Bar(
	}
}", options: Constants.ParseOptions);
			var (node, model) = DotnetStyleQualificationForMethodStyleTests.GetInformation(unit, "Bar");
			var newStyle = style.Update(new ModelNodeInformation<InvocationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
