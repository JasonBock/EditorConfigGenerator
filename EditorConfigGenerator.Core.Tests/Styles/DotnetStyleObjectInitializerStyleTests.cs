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
	public static class DotnetStyleObjectInitializerStyleTests
	{
		[Test]
		public static void CreateWithCustomSeverity()
		{
			const Severity suggestion = Severity.Suggestion;
			var data = new BooleanData();
			var style = new DotnetStyleObjectInitializerStyle(data, suggestion);
			Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
		}

		[Test]
		public static void CreateWithNoData()
		{
			var data = new BooleanData();
			var style = new DotnetStyleObjectInitializerStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreFalseData()
		{
			var data = new BooleanData(1u, 0u, 1u);
			var style = new DotnetStyleObjectInitializerStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStyleObjectInitializerStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTrueData()
		{
			var data = new BooleanData(1u, 1u, 0u);
			var style = new DotnetStyleObjectInitializerStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(
				$"{DotnetStyleObjectInitializerStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new DotnetStyleObjectInitializerStyle(new BooleanData(3u, 1u, 2u));
			var style2 = new DotnetStyleObjectInitializerStyle(new BooleanData(30u, 10u, 20u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData());
			Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new DotnetStyleObjectInitializerStyle(data);

			Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		private static (ObjectCreationExpressionSyntax, SemanticModel) GetInformation(CompilationUnitSyntax unit)
		{
			var invocation = unit.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().First();
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (invocation, model);
		}

		[Test]
		public static void UpdateUsingInitializer()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data { Value = 22 };
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateObjectCreationAsSingleStatement()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data();
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateObjectCreationAsLastStatement()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var a = 22;
		var b = 33 + a;
		var x = new Data();
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateSetPropertyOnSameObject()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data();
		x.Value = 22;
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateCallMethodOnAssignedObject()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data 
{ 
	public void Quux() { }
	public int Value { get; set; } 
} 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data();
		x.Quux();
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateSetPropertyOnDifferentObject()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var different = new Data();
		var x = new Data();
		different.Value = 22;
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateSetPropertyOnObjectThatIsNotAssigned()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));
			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public Data D
	{
		get { return new Data().Value; }
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new DotnetStyleObjectInitializerStyle(new BooleanData(default, default, default));

			var unit = SyntaxFactory.ParseCompilationUnit(
@"public class Data { public int Value { get; set; } } 

public class Foo 
{ 
	public void Bar()
	{
		var x = new Data=> { Value = 22 };
	}
}", options: Shared.ParseOptions);
			var (node, model) = DotnetStyleObjectInitializerStyleTests.GetInformation(unit);
			var newStyle = style.Update(new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, model));

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}
	}
}
