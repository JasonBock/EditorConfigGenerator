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

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new TabSpaceData(3, 1, 2);
			var data2 = new TabSpaceData(3, 2, 1);
			var data3 = new TabSpaceData(3, 1, 2);

			Assert.That(data1, Is.Not.EqualTo(data2));
			Assert.That(data1, Is.EqualTo(data3));
			Assert.That(data2, Is.Not.EqualTo(data3));

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.That(data1 == data1, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.That(data1 == data2, Is.False);
			Assert.That(data1 == data3, Is.True);
			Assert.That(data2 == data3, Is.False);
			Assert.That((null as TabSpaceData) == data1, Is.False);
			Assert.That(data1 == (null as TabSpaceData), Is.False);

			Assert.That(data1 != data2, Is.True);
			Assert.That(data1 != data3, Is.False);
			Assert.That(data2 != data3, Is.True);
		}

		[Test]
		public static void VerifyToString() =>
			Assert.That(new TabSpaceData(3, 1, 2).ToString(), 
				Is.EqualTo("TotalOccurences = 3, TabOccurences = 1, SpaceOccurences = 2"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new TabSpaceData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new TabSpaceData(3, 1, 2);
			var data2 = new TabSpaceData(3, 2, 1);
			var data3 = new TabSpaceData(3, 1, 2);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}
