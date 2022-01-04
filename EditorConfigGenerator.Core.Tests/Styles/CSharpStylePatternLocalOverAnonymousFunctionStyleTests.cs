using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles
{
   [TestFixture]
	public static class CSharpStylePatternLocalOverAnonymousFunctionStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{CSharpStylePatternLocalOverAnonymousFunctionStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{CSharpStylePatternLocalOverAnonymousFunctionStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(3u, 1u, 2u));
			var style2 = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(30u, 10u, 20u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData());
			Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(data);

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
		public static void UpdateWithLocalFunction()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public void Bar()
	{
		int X() => 22;
	}
}", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<LocalFunctionStatementSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDelegateDeclaration()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public delegate void X();

public class Foo
{
	public void Bar()
	{
		X x = Bar;
	}
}", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<VariableDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithActionDeclaration()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo
{
	public void Bar()
	{
		Action x = () => { };
	}
}", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<VariableDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithFuncDeclaration()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"using System;

public class Foo
{
	public void Bar()
	{
		Func<int> x = () => 22;
	}
}", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<VariableDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithUnexpectedNodeType()
		{
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit("public class Foo { }", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<ClassDeclarationSyntax>(unit);
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
			var style = new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit("public class -Foo { }", options: Shared.ParseOptions);
			var (node, model) = CSharpStylePatternLocalOverAnonymousFunctionStyleTests.GetInformation<ClassDeclarationSyntax>(unit);
			var newStyle = style.Update(new ModelNodeInformation<SyntaxNode>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
