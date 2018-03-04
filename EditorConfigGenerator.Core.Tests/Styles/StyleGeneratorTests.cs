using EditorConfigGenerator.Core.Styles;
using NUnit.Framework;
using System;
using System.IO;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
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
				Assert.That(StyleGenerator.GenerateFromSourceFile(sourceFile), 
					Is.EqualTo("[*.cs]\r\ncsharp_style_var_for_built_in_types = true:error\r\n"));
			}
			finally
			{
				File.Delete(sourceFile);
			}
		}
	}
}
