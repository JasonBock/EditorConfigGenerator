using Buildalyzer;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace EditorConfigGenerator.Core.Styles
{
	public static class StyleGenerator
	{
		public static string GenerateFromSolution(string solutionFile)
		{
			var manager = new AnalyzerManager(solutionFile);
			var walker = new StyleWalker();

			foreach (var project in manager.Projects)
			{
				foreach (var sourceFile in project.Value.GetSourceFiles())
				{
					if (Path.GetExtension(sourceFile).ToLower() == ".cs")
					{
						var sourceWalker = new StyleWalker(); 
						sourceWalker.Visit(SyntaxFactory.ParseCompilationUnit(
							File.ReadAllText(sourceFile)));
						walker = walker.Add(sourceWalker);
					}
				}
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromProject(string projectFile)
		{
			var manager = new AnalyzerManager();
			var project = manager.GetProject(projectFile);
			var walker = new StyleWalker();

			foreach (var sourceFile in project.GetSourceFiles())
			{
				if (Path.GetExtension(sourceFile).ToLower() == ".cs")
				{
					var sourceWalker = new StyleWalker();
					sourceWalker.Visit(SyntaxFactory.ParseCompilationUnit(
						File.ReadAllText(sourceFile)));
					walker = walker.Add(sourceWalker);
				}
			}

			return walker.GenerateConfiguration();
		}

		public static string GenerateFromSourceFile(string sourceFile)
		{
			var walker = new StyleWalker();

			if (Path.GetExtension(sourceFile).ToLower() == ".cs")
			{
				var sourceWalker = new StyleWalker();
				sourceWalker.Visit(SyntaxFactory.ParseCompilationUnit(
					File.ReadAllText(sourceFile)));
				walker = walker.Add(sourceWalker);
			}

			return walker.GenerateConfiguration();
		}
	}
}
