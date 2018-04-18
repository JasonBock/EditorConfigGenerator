using Microsoft.CodeAnalysis.CSharp;

namespace EditorConfigGenerator.Core.Tests
{
	internal static class Constants
	{
		internal static readonly CSharpParseOptions ParseOptions =
			new CSharpParseOptions(languageVersion: LanguageVersion.Latest);
	}
}
