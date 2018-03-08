using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using Spackle;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class TabSpaceDataTests
	{
		[Test]
		public static void Create()
		{
			var generator = new RandomObjectGenerator();
			var totalOccurences = generator.Generate<uint>();
			var tabOccurences = generator.Generate<uint>();
			var spaceOccurences = generator.Generate<uint>();

			var data = new TabSpaceData(totalOccurences, tabOccurences, spaceOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(tabOccurences), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(spaceOccurences), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new TabSpaceData();
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(default(uint)), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(default(uint)), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void Add()
		{
			var data1 = new TabSpaceData(3u, 1u, 2u);
			var data2 = new TabSpaceData(30u, 10u, 20u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(33u), nameof(data3.TotalOccurences));
			Assert.That(data3.TabOccurences, Is.EqualTo(11u), nameof(data3.TabOccurences));
			Assert.That(data3.SpaceOccurences, Is.EqualTo(22u), nameof(data3.SpaceOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new TabSpaceData();
			Assert.That(() => data.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithTab()
		{
			var data = new TabSpaceData(3u, 1u, 2u);
			data = data.Update(true);

			Assert.That(data.TotalOccurences, Is.EqualTo(4u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(2u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(2u), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void UpdateWithSpace()
		{
			var data = new TabSpaceData(3u, 1u, 2u);
			data = data.Update(false);

			Assert.That(data.TotalOccurences, Is.EqualTo(4u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(1u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(3u), nameof(data.SpaceOccurences));
		}
	}
}
