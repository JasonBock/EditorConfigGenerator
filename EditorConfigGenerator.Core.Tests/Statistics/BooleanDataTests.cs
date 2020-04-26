using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class BooleanDataTests
	{
		[Test]
		public static void Create()
		{
			var totalOccurences = 3u;
			var trueOccurences = 1u;
			var falseOccurences = 2u;

			var data = new BooleanData(totalOccurences, trueOccurences, falseOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(trueOccurences), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(falseOccurences), nameof(data.FalseOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new BooleanData();
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(default(uint)), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(default(uint)), nameof(data.FalseOccurences));
		}

		[Test]
		public static void EvaluateConsistency() => 
			Assert.That(new BooleanData(100u, 75u, 25u).Consistency, Is.EqualTo(0.5f));

		[Test]
		public static void Add()
		{
			var data1 = new BooleanData(3u, 1u, 2u);
			var data2 = new BooleanData(30u, 10u, 20u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(33u), nameof(data3.TotalOccurences));
			Assert.That(data3.TrueOccurences, Is.EqualTo(11u), nameof(data3.TrueOccurences));
			Assert.That(data3.FalseOccurences, Is.EqualTo(22u), nameof(data3.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new BooleanData();
			Assert.That(() => data.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithTrue()
		{
			var data = new BooleanData(3u, 1u, 2u);
			data = data.Update(true);

			Assert.That(data.TotalOccurences, Is.EqualTo(4u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(2u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(2u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithFalse()
		{
			var data = new BooleanData(3u, 1u, 2u);
			data = data.Update(false);

			Assert.That(data.TotalOccurences, Is.EqualTo(4u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(3u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new BooleanData(3, 1, 2);
			var data2 = new BooleanData(3, 2, 1);
			var data3 = new BooleanData(3, 1, 2);

			Assert.That(data1, Is.Not.EqualTo(data2));
			Assert.That(data1, Is.EqualTo(data3));
			Assert.That(data2, Is.Not.EqualTo(data3));

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.That(data1 == data1, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.That(data1 == data2, Is.False);
			Assert.That(data1 == data3, Is.True);
			Assert.That(data2 == data3, Is.False);
			Assert.That((null as BooleanData)! == data1, Is.False);
			Assert.That(data1 == (null as BooleanData)!, Is.False);

			Assert.That(data1 != data2, Is.True);
			Assert.That(data1 != data3, Is.False);
			Assert.That(data2 != data3, Is.True);
		}

		[Test]
		public static void VerifyToString() =>
			Assert.That(new BooleanData(3, 1, 2).ToString(), 
				Is.EqualTo("TotalOccurences = 3, TrueOccurences = 1, FalseOccurences = 2"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new BooleanData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new BooleanData(3, 1, 2);
			var data2 = new BooleanData(3, 2, 1);
			var data3 = new BooleanData(3, 1, 2);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));

			data1 = new BooleanData(43, 20, 23);
			data2 = new BooleanData(43, 23, 20);
			data3 = new BooleanData(43, 20, 23);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));

			data1 = new BooleanData(112, 111, 1);
			data2 = new BooleanData(111, 110, 1);
			data3 = new BooleanData(112, 111, 1);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}
