using EditorConfigGenerator.Core.Styles;
using System;
using System.IO;

namespace EditorConfigGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			if(args?.Length != 1)
			{
				Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
			}
			else
			{
				var file = args[0];
				var extension = Path.GetExtension(file);

				if (extension == ".sln")
				{
					Console.Out.WriteLine(StyleGenerator.GenerateFromSolution(file));
				}
				else if (extension == ".csproj")
				{
					Console.Out.WriteLine(StyleGenerator.GenerateFromProject(file));
				}
				else if (extension == ".cs")
				{
					Console.Out.WriteLine(StyleGenerator.GenerateFromSourceFile(file));
				}
				else
				{
					Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
				}
			}
		}
	}
}
