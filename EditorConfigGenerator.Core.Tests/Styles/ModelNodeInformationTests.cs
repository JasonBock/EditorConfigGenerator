using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles
{
   [TestFixture]
	public static class ModelNodeInformationTests
	{
		private static readonly CompilationUnitSyntax unit = SyntaxFactory.ParseCompilationUnit("public class F { }", options: Shared.ParseOptions);

		[Test]
		public static void Create()
		{
			var tree = ModelNodeInformationTests.unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			var information = new ModelNodeInformation<CompilationUnitSyntax>(
				ModelNodeInformationTests.unit, model);
			Assert.That(information.Node, Is.SameAs(ModelNodeInformationTests.unit), nameof(information.Node));
			Assert.That(information.Model, Is.SameAs(model), nameof(information.Model));
		}

		[Test]
		public static void CreateWithNullNode()
		{
			var tree = ModelNodeInformationTests.unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);
			Assert.That(() => new ModelNodeInformation<IdentifierNameSyntax>(null!, model), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void CreateWithNullModel() =>
			Assert.That(() => new ModelNodeInformation<CompilationUnitSyntax>(ModelNodeInformationTests.unit, null!), Throws.TypeOf<ArgumentNullException>());

		[Test]
		public static void DeconstructToTuple()
		{
			var tree = ModelNodeInformationTests.unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			var information = new ModelNodeInformation<CompilationUnitSyntax>(
				ModelNodeInformationTests.unit, model);
			var (newNode, newModel) = information;
			Assert.That(newNode, Is.SameAs(information.Node), nameof(information.Node));
			Assert.That(newModel, Is.SameAs(information.Model), nameof(information.Model));
		}
	}
}
