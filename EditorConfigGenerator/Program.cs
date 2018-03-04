using EditorConfigGenerator.Core.Styles;
using System;
using System.CommandLine;
using System.Linq;

namespace EditorConfigGenerator
{
	class Program
	{
		private const string SolutionName = "s";
		private const string ProjectName = "p";
		private const string SourceFileName = "f";

		static void Main(string[] args)
		{
			var value = string.Empty;

			var result = ArgumentSyntax.Parse(args, syntax =>
			{
				syntax.DefineOption(Program.SolutionName, ref value, false, "A .sln file");
				syntax.DefineOption(Program.ProjectName, ref value, false, "A .csproj file");
				syntax.DefineOption(Program.SourceFileName, ref value, false, "A .cs file");
			});

			var options = result.GetActiveArguments().Where(_ => _.IsSpecified).ToArray();

			if (options.Length != 1)
			{
				Console.Out.WriteLine(result.GetHelpText());
			}
			else
			{
				var option = options[0];
				var optionValue = (string)option.Value;

				switch (option.Name)
				{
					case Program.SolutionName:
					{
						Console.Out.WriteLine(StyleGenerator.GenerateFromSolution(optionValue));
						break;
					}
					case Program.ProjectName:
					{
						Console.Out.WriteLine(StyleGenerator.GenerateFromProject(optionValue));
						break;
					}
					case Program.SourceFileName:
					{
						Console.Out.WriteLine(StyleGenerator.GenerateFromSourceFile(optionValue));
						break;
					}
				}
			}
		}
	}
}
