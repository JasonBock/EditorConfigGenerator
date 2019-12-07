using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static async Task<string> Generate(DirectoryInfo directory, TextWriter writer, bool generateStatistics = false)
		{
			var aggregator = new StyleAggregator();

			async Task AnalyzeFilesAsync(DirectoryInfo rootDirectory)
			{
				writer.WriteLine($"Analyzing {rootDirectory}...");

				foreach (var file in rootDirectory.EnumerateFiles())
				{
					if (file.Extension.ToLower() == ".cs")
					{
						writer.WriteLine($"\tAnalyzing {file.FullName}...");
						var (unit, model) = StyleGenerator.GetCompilationInformation(file.FullName);
						aggregator = aggregator.Update(new StyleAggregator().Add(unit, model));
					}
				}

				foreach (var subDirectory in rootDirectory.EnumerateDirectories())
				{
					await AnalyzeFilesAsync(subDirectory);
				}
			}

			await AnalyzeFilesAsync(directory);

			if(generateStatistics)
			{
				using var statisticsWriter = File.CreateText("statistics.json");
				new JsonSerializer().Serialize(statisticsWriter, aggregator);
			}

			return aggregator.GenerateConfiguration();
		}

		public static string GenerateFromDocument(FileInfo document, TextWriter writer, bool generateStatistics = false)
		{
			if (document.Extension.ToLower() == ".cs")
			{
				writer.WriteLine($"Analyzing {document.FullName}...");
				var (unit, model) = StyleGenerator.GetCompilationInformation(document.FullName);
				var aggregator = new StyleAggregator().Add(unit, model);

				if (generateStatistics)
				{
					using var statisticsWriter = File.CreateText("statistics.json");
					new JsonSerializer().Serialize(statisticsWriter, aggregator);
				}

				return aggregator.GenerateConfiguration();
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