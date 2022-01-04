using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Collections.Immutable;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace EditorConfigGenerator.Core.Tests
{
	internal static class Shared
	{
		internal static readonly CSharpParseOptions ParseOptions =
			new CSharpParseOptions(languageVersion: LanguageVersion.Latest);

		internal static void VerifyModifiers(ImmutableDictionary<string, (uint weight, uint frequency)> modifiers,
			params KeyValuePair<string, (uint weight, uint frequency)>[] pairs)
		{
			foreach (var pair in pairs)
			{
				foreach (var modifier in modifiers)
				{
					if (modifier.Key == pair.Key)
					{
						Assert.That(modifier.Value, Is.EqualTo(pair.Value), pair.Key);
					}
					else
					{
						Assert.That(modifier.Value, Is.EqualTo((0u, 0u)), modifier.Key);
					}
				}
			}
		}
	}
}
