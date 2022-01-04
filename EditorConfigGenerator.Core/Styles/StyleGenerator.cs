using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace EditorConfigGenerator.Core.Styles;

public static class StyleGenerator
{
	public static async Task<string> Generate(DirectoryInfo directory, TextWriter writer, bool generateStatistics = false)
	{
		if (directory is null)
		{
			throw new ArgumentNullException(nameof(directory));
		}

		if (writer is null)
		{
			throw new ArgumentNullException(nameof(writer));
		}

		var aggregator = new StyleAggregator();

		await writer.WriteLineAsync($"Analyzing {directory}...").ConfigureAwait(false);

		foreach (var file in directory.EnumerateFiles("*.cs", SearchOption.AllDirectories))
		{
			await writer.WriteLineAsync($"\tAnalyzing {file.FullName}...").ConfigureAwait(false);
			var (unit, model) = StyleGenerator.GetCompilationInformation(file.FullName);
			aggregator = aggregator.Update(new StyleAggregator().Add(unit, model));
		}

		if (generateStatistics)
		{
			using var statisticsWriter = File.CreateText("statistics.json");
			new JsonSerializer().Serialize(statisticsWriter, aggregator);
		}

		return aggregator.GenerateConfiguration();
	}

	public static string GenerateFromDocument(FileInfo document, TextWriter writer, bool generateStatistics = false)
	{
		if (document is null)
		{
			throw new ArgumentNullException(nameof(document));
		}

		if (writer is null)
		{
			throw new ArgumentNullException(nameof(writer));
		}

		if (document.Extension.ToUpperInvariant() == ".CS")
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