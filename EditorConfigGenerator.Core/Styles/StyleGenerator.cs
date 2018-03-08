using Buildalyzer;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static string GenerateFromSolution(string solutionFile, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {Path.GetFileName(solutionFile)}...");
			var manager = new AnalyzerManager(solutionFile);
			var walker = new StyleWalker();

			foreach (var project in manager.Projects)
			{
				writer.WriteLine($"\tAnalyzing {Path.GetFileName(project.Value.ProjectFilePath)}...");
				foreach (var sourceFile in project.Value.GetSourceFiles())
				{
					writer.WriteLine($"\t\tAnalyzing {Path.GetFileName(sourceFile)}...");
					StyleGenerator.ProcessSourceFile(sourceFile, walker);
				}
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromProject(string projectFile, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {Path.GetFileName(projectFile)}...");
			var manager = new AnalyzerManager();
			var project = manager.GetProject(projectFile);
			var walker = new StyleWalker();

			foreach (var sourceFile in project.GetSourceFiles())
			{
				writer.WriteLine($"\tAnalyzing {Path.GetFileName(sourceFile)}...");
				StyleGenerator.ProcessSourceFile(sourceFile, walker);
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromSourceFile(string sourceFile, TextWriter writer)
		{
			writer.WriteLine($"Analyzing {Path.GetFileName(sourceFile)}...");
			var walker = new StyleWalker();
			StyleGenerator.ProcessSourceFile(sourceFile, walker);
			return walker.GenerateConfiguration();
		}

		private static void ProcessSourceFile(string sourceFile, StyleWalker walker)
		{
			if (Path.GetExtension(sourceFile).ToLower() == ".cs")
			{
				walker.Visit(SyntaxFactory.ParseCompilationUnit(
					File.ReadAllText(sourceFile)));
			}
		}
	}
}