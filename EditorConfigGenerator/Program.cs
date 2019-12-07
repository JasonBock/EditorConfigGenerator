using EditorConfigGenerator.Core.Styles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator
{
	class Program
	{
		static async Task Main(DirectoryInfo? directory, FileInfo? file, bool generateStatistics = false)
		{
			if(directory is { })
			{
				Console.Out.WriteLine(await StyleGenerator.Generate(directory, Console.Out, generateStatistics));
			}
			else
			{
				if (file is { } && file.Extension == ".cs")
				{
					Console.Out.WriteLine(StyleGenerator.GenerateFromDocument(file, Console.Out, generateStatistics));
				}
				else
				{
					Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
				}
			}
		}
	}
}
