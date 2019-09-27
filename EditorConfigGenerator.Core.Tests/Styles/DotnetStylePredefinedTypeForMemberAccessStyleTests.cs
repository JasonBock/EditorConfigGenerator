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
	public static class DotnetStylePredefinedTypeForMemberAccessStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStylePredefinedTypeForMemberAccessStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStylePredefinedTypeForMemberAccessStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData());
			Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(data);

			Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		private static (MemberAccessExpressionSyntax, SemanticModel) GetInformation(CompilationUnitSyntax unit)
		{
			var invocation = unit.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First();
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (invocation, model);
		}

		[Test]
		public static void UpdateWithAccessThatIsPredefinedTypeUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public int Bar() => int.MaxValue;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForMemberAccessStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<MemberAccessExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithAccessThatIsPredefinedTypeNotUsingCSharpKeyword()
		{
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo 
{ 
	public int Bar() => Int32.MaxValue;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForMemberAccessStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<MemberAccessExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithAccessThatIsNotPredefinedType()
		{
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo 
{ 
	public int BigValue = 0;

	public int Bar() => Foo.BigValue;
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForMemberAccessStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<MemberAccessExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		var x = int.MaxValue =>;
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStylePredefinedTypeForMemberAccessStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<MemberAccessExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
