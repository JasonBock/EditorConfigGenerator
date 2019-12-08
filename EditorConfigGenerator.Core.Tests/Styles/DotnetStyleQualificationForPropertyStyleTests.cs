using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class DotnetStyleQualificationForPropertyStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new DotnetStyleQualificationForPropertyStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new DotnetStyleQualificationForPropertyStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new DotnetStyleQualificationForPropertyStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStyleQualificationForPropertyStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new DotnetStyleQualificationForPropertyStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStyleQualificationForPropertyStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new DotnetStyleQualificationForPropertyStyle(new BooleanData(3u, 1u, 2u));
			var style2 = new DotnetStyleQualificationForPropertyStyle(new BooleanData(30u, 10u, 20u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData());
			Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new DotnetStyleQualificationForPropertyStyle(data);

			Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		private static (T, SemanticModel) GetInformation<T>(CompilationUnitSyntax unit)
			where T : SyntaxNode
		{
			var invocation = unit.DescendantNodes().OfType<T>().Last();
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (invocation, model);
		}

		[Test]
		public static void UpdateWithInstancePropertyQualifiedWithThis()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public int X { get; set; }

	public int Bar() => this.X;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<MemberAccessExpressionSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithInstancePropertyNotQualifiedWithThis()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public int X { get; set; }

	public int Bar() => X;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<IdentifierNameSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithInstancePropertyQualifiedWithVariableName()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data
{
	public int Value { get; set; }
}

public class Foo
{
	public int Bar() 
	{
		var q = new Data();
		return q.Value;
	};
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<MemberAccessExpressionSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithStaticPropertyWithClassName()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public static int X { get; set; }

	public int Bar() => Foo.X;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<MemberAccessExpressionSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithStaticPropertyWithoutClassName()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public static int X { get; set; }

	public int Bar() => X;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<IdentifierNameSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithUnexpectedNodeType()
		{
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public event EventHandler DoIt;

	public void Bar()
	{
		this.DoIt += (a, b) => { };
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<IdentifierNameSyntax>(unit);
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
			var style = new DotnetStyleQualificationForPropertyStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public int X { get; set; }

	public void Bar()
	{
		var q = this.X =>;
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleQualificationForPropertyStyleTests.GetInformation<MemberAccessExpressionSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
