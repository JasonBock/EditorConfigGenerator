using EditorConfigGenerator.Core.Styles;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles
{
   [TestFixture]
	public static class StyleGeneratorTests
	{
		[Test]
		public static void GenerateFromFile()
		{
			var sourceFile = new FileInfo($"{Guid.NewGuid().ToString("N")}.cs");

			try
			{
				File.WriteAllText(sourceFile.FullName,
	@"public static class Test
{
	public static void VarFoo()
	{
		var a = 1;
	}
}");
				using var writer = new StringWriter();
				Assert.That(string.IsNullOrWhiteSpace(StyleGenerator.GenerateFromDocument(sourceFile, writer)), Is.False);
			}
			finally
			{
				File.Delete(sourceFile.FullName);
			}
		}
	}
}
