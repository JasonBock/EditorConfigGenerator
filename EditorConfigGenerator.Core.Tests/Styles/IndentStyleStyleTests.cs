using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class IndentStyleStyleTests
	{
		[Test]
		public static void CreateWithNoData()
		{
			var data = new TabSpaceData();
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreSpaceData()
		{
			var data = new TabSpaceData(1u, 0u, 1u);
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("indent_style = space"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTabData()
		{
			var data = new TabSpaceData(1u, 1u, 0u);
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("indent_style = tab"), nameof(style.GetSetting));
		}
	}
}
