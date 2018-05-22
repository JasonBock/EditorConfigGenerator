using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class EnumExtensionsTests
	{
		[Test]
		public static void GetDescription() =>
			Assert.That(TestWithDescriptions.Two.GetDescription(), Is.EqualTo("number two"));

		[Test]
		public static void GetDescriptionWhenValueHasNoDescriptionAttribute() =>
			Assert.That(TestWithoutDescriptions.Two.GetDescription(), Is.EqualTo("Two"));
	}
}
