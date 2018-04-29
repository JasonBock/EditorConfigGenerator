using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static async Task<string> GenerateFromDirectoryAsync(string directory, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {directory}...");
			var aggregator = new StyleAggregator();

			async Task AnalyzeFilesAsync(string rootDirectory)
			{
				foreach (var file in Directory.GetFiles(rootDirectory))
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

				foreach (var subDirectory in Directory.GetDirectories(rootDirectory))
				{
					await AnalyzeFilesAsync(subDirectory);
				}
			}

			await AnalyzeFilesAsync(directory);
			return aggregator.GenerateConfiguration();
		}

		public static async Task<string> GenerateFromSolutionAsync(string solutionFile, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {Path.GetFileName(solutionFile)}...");
			var manager = new AnalyzerManager(solutionFile);
			var solution = manager.GetWorkspace().CurrentSolution;
			var aggregator = new StyleAggregator();

			foreach (var project in solution.Projects)
			{
				var compilation = await project.GetCompilationAsync();

				writer.WriteLine($"\tAnalyzing {Path.GetFileName(project.FilePath)}...");
				foreach (var document in project.Documents)
				{
					writer.WriteLine($"\t\tAnalyzing {Path.GetFileName(document.FilePath)}...");

					if (Path.GetExtension(document.FilePath).ToLower() == ".cs")
					{
						var root = await document.GetSyntaxRootAsync();
						var model = compilation.GetSemanticModel(root.SyntaxTree);
						aggregator = aggregator.Update(new StyleAggregator().Add(root as CompilationUnitSyntax, model));
					}
				}
			}

			return aggregator.GenerateConfiguration();
		}

		public static async Task<string> GenerateFromProjectAsync(string projectFile, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {Path.GetFileName(projectFile)}...");
			var manager = new AnalyzerManager();
			var project = manager.GetProject(projectFile).GetWorkspace().CurrentSolution.Projects.ToArray()[0];
			var compilation = await project.GetCompilationAsync();
			var aggregator = new StyleAggregator();

			foreach (var document in project.Documents)
			{
				writer.WriteLine($"\tAnalyzing {Path.GetFileName(document.FilePath)}...");

				if (Path.GetExtension(document.FilePath).ToLower() == ".cs")
				{
					var root = await document.GetSyntaxRootAsync();
					var model = compilation.GetSemanticModel(root.SyntaxTree);
					aggregator = aggregator.Update(new StyleAggregator().Add(root as CompilationUnitSyntax, model));
				}
			}

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