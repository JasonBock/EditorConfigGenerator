using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static async Task<string> Generate(string directory, TextWriter writer)
		{
			var aggregator = new StyleAggregator();

			async Task AnalyzeFilesAsync(string rootDirectory)
			{
				writer.WriteLine($"Analyzing {rootDirectory}...");

				foreach (var file in Directory.EnumerateFiles(rootDirectory))
				{
					if (Path.GetExtension(file).ToLower() == ".cs")
					{
						writer.WriteLine($"\tAnalyzing {Path.GetFileName(file)}...");
						var (unit, model) = StyleGenerator.GetCompilationInformation(file);
						aggregator = aggregator.Update(new StyleAggregator().Add(unit, model));
					}
				}

				foreach (var subDirectory in Directory.EnumerateDirectories(rootDirectory))
				{
					await AnalyzeFilesAsync(subDirectory);
				}
			}

			await AnalyzeFilesAsync(directory);
			return aggregator.GenerateConfiguration();
		}

		public static string GenerateFromDocument(string document, TextWriter writer)
		{
			if (Path.GetExtension(document).ToLower() == ".cs")
			{
				writer.WriteLine($"Analyzing {Path.GetFileName(document)}...");
				var (unit, model) = StyleGenerator.GetCompilationInformation(document);
				return new StyleAggregator().Add(unit, model).GenerateConfiguration();
			}

			return string.Empty;
		}

		private static (CompilationUnitSyntax, SemanticModel) GetCompilationInformation(string document)
		{
			var unit = SyntaxFactory.ParseCompilationUnit(File.ReadAllText(document));
			var tree = unit.SyntaxTree;
			var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
				new[] { tree },
				new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
			var model = compilation.GetSemanticModel(tree);

			return (unit, model);
		}
	}
}