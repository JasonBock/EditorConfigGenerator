using EditorConfigGenerator.Core.Styles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator
{
	class Program
	{
		static async Task Main(string[] args)
		{
			if (args?.Length != 1)
			{
				Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
			}
			else
			{
				var file = args[0];
				var extension = Path.GetExtension(file);

				if (extension == ".sln")
				{
					Console.Out.WriteLine(await StyleGenerator.GenerateFromSolutionAsync(file, Console.Out));
				}
				else if (extension == ".csproj")
				{
					Console.Out.WriteLine(await StyleGenerator.GenerateFromProjectAsync(file, Console.Out));
				}
				else if (extension == ".cs")
				{
					Console.Out.WriteLine(StyleGenerator.GenerateFromDocument(file, Console.Out));
				}
				else
				{
					Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
				}
			}
		}
	}
}
