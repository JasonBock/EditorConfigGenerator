using Buildalyzer;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static string GenerateFromSolution(string solutionFile)
		{
			Console.Out.WriteLine($"Analyzing {solutionFile}...");
			var manager = new AnalyzerManager(solutionFile);
			var walker = new StyleWalker();

			foreach (var project in manager.Projects)
			{
				Console.Out.WriteLine($"\tAnalyzing {project.Value.ProjectFilePath}...");
				foreach (var sourceFile in project.Value.GetSourceFiles())
				{
					Console.Out.WriteLine($"\t\tAnalyzing {sourceFile}...");
					StyleGenerator.ProcessSourceFile(sourceFile, walker);
				}
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromProject(string projectFile)
		{
			Console.Out.WriteLine($"Analyzing {projectFile}...");
			var manager = new AnalyzerManager();
			var project = manager.GetProject(projectFile);
			var walker = new StyleWalker();

			foreach (var sourceFile in project.GetSourceFiles())
			{
				Console.Out.WriteLine($"\tAnalyzing {sourceFile}...");
				StyleGenerator.ProcessSourceFile(sourceFile, walker);
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromSourceFile(string sourceFile)
		{
			Console.Out.WriteLine($"Analyzing {sourceFile}...");
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
