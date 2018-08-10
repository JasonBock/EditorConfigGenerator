using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static async Task<string> Generate(string directory, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {directory}...");
			var aggregator = new StyleAggregator();

			async Task AnalyzeFilesAsync(string rootDirectory)
			{
				foreach (var file in Directory.EnumerateFiles(rootDirectory))
				{
					if (Path.GetExtension(file).ToLower() == ".cs")
					{
						Console.Out.WriteLine($"\tAnalyzing {file}...");
						var unit = SyntaxFactory.ParseCompilationUnit(File.ReadAllText(file));
						var tree = unit.SyntaxTree;
						var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
							new[] { tree },
							new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
						var model = compilation.GetSemanticModel(tree);
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
			writer.WriteLine($"Analyzing {Path.GetFileName(document)}...");

			if (Path.GetExtension(document).ToLower() == ".cs")
			{
				var unit = SyntaxFactory.ParseCompilationUnit(File.ReadAllText(document));
				var tree = unit.SyntaxTree;
				var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString("N"),
					new[] { tree },
					new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
				var model = compilation.GetSemanticModel(tree);

				return new StyleAggregator().Add(unit, model).GenerateConfiguration();
			}

			return string.Empty;
		}
	}
}