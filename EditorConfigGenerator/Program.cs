using EditorConfigGenerator.Core.Styles;

namespace EditorConfigGenerator
{
	public static class Program
	{
		public static async Task Main(DirectoryInfo? directory, FileInfo? file, bool generateStatistics = false)
		{
			if (directory is not null)
			{
				await Console.Out.WriteLineAsync(await StyleGenerator.Generate(directory, Console.Out, generateStatistics).ConfigureAwait(false)).ConfigureAwait(false);
			}
			else
			{
				if (file is not null && file.Extension == ".cs")
				{
					await Console.Out.WriteLineAsync(StyleGenerator.GenerateFromDocument(file, Console.Out, generateStatistics)).ConfigureAwait(false);
				}
				else
				{
					await Console.Out.WriteLineAsync("Usage: {fileName}, where the extension is .sln, .csproj, or .cs").ConfigureAwait(false);
				}
			}
		}
	}
}
