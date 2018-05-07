using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class EnumExtensionsTests
	{
		[Test]
		public static void GetDescription() =>
			Assert.That(TestDescription.Two.GetDescription(), Is.EqualTo("two"));
	}
}
