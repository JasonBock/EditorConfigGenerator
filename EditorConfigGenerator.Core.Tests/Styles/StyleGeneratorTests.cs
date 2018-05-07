using EditorConfigGenerator.Core.Styles;
using NUnit.Framework;
using System;
using System.IO;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	[Parallelizable(ParallelScope.Self)]
	public static class StyleGeneratorTests
	{
		[Test]
		public static void GenerateFromFile()
		{
			var sourceFile = $"{Guid.NewGuid().ToString("N")}.cs";

			try
			{
				File.WriteAllText(sourceFile,
	@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}");
				using (var writer = new StringWriter())
				{
					Assert.That(string.IsNullOrWhiteSpace(StyleGenerator.GenerateFromDocument(sourceFile, writer)), Is.False);
				}
			}
			finally
			{
				File.Delete(sourceFile);
			}
		}
	}
}
