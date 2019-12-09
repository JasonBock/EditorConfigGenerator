using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	public static class ExpressionBodiedDataTests
	{
		[Test]
		public static void Create()
		{
			var totalOccurences = 10u;
			var arrowSingleLineOccurences = 5u;
			var arrowMultiLineOccurences = 3u;
			var blockOccurences = 2u;

			var data = new ExpressionBodiedData(totalOccurences, arrowSingleLineOccurences, arrowMultiLineOccurences,
				blockOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(arrowSingleLineOccurences), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(arrowMultiLineOccurences), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(blockOccurences), nameof(data.BlockOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new ExpressionBodiedData();
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(default(uint)), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(default(uint)), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(default(uint)), nameof(data.BlockOccurences));
		}

		[Test]
		public static void ExpressionBodiedData() =>
			Assert.That(new ExpressionBodiedData(100u, 70u, 20u, 10u).Consistency, Is.EqualTo(0.800000012f));

		[Test]
		public static void Add()
		{
			var data1 = new ExpressionBodiedData(10u, 5u, 3u, 2u);
			var data2 = new ExpressionBodiedData(100u, 50u, 30u, 20u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(110u), nameof(data3.TotalOccurences));
			Assert.That(data3.ArrowSingleLineOccurences, Is.EqualTo(55u), nameof(data3.ArrowSingleLineOccurences));
			Assert.That(data3.ArrowMultiLineOccurences, Is.EqualTo(33u), nameof(data3.ArrowMultiLineOccurences));
			Assert.That(data3.BlockOccurences, Is.EqualTo(22u), nameof(data3.BlockOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new ExpressionBodiedData();
			Assert.That(() => data.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void GetSettingWithNoOccurrences()
		{
			var data = new ExpressionBodiedData(0u, 0u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo(string.Empty), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithArrows()
		{
			var data = new ExpressionBodiedData(2u, 1u, 1u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithBlocks()
		{
			var data = new ExpressionBodiedData(1u, 0u, 0u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = false:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithSingleLine()
		{
			var data = new ExpressionBodiedData(4u, 3u, 1u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithFavoritingArrows()
		{
			var data = new ExpressionBodiedData(5u, 3u, 1u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithFavoritingBlocks()
		{
			var data = new ExpressionBodiedData(4u, 1u, 0u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = false:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWhenOnSingleLine()
		{
			var data = new ExpressionBodiedData(4u, 1u, 2u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = when_on_single_line:error"), nameof(data.GetSetting));
		}

		[TestCase(ExpressionBodiedDataOccurence.ArrowSingleLine, 1u, 0u, 0u)]
		[TestCase(ExpressionBodiedDataOccurence.ArrowMultiLine, 0u, 1u, 0u)]
		[TestCase(ExpressionBodiedDataOccurence.Block, 0u, 0u, 1u)]
		public static void Update(ExpressionBodiedDataOccurence occurrence, uint expectedArrowSingleLineOccurences,
			uint expectedArrowMultiLineOccurences, uint expectedBlockOccurences)
		{
			var data = new ExpressionBodiedData();
			data = data.Update(occurrence);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(expectedArrowSingleLineOccurences), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(expectedArrowMultiLineOccurences), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(expectedBlockOccurences), nameof(data.BlockOccurences));
		}

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new ExpressionBodiedData(6, 1, 2, 3);
			var data2 = new ExpressionBodiedData(6, 2, 1, 3);
			var data3 = new ExpressionBodiedData(6, 1, 2, 3);

			Assert.That(data1, Is.Not.EqualTo(data2));
			Assert.That(data1, Is.EqualTo(data3));
			Assert.That(data2, Is.Not.EqualTo(data3));

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.That(data1 == data1, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.That(data1 == data2, Is.False);
			Assert.That(data1 == data3, Is.True);
			Assert.That(data2 == data3, Is.False);
			Assert.That((null as ExpressionBodiedData)! == data1, Is.False);
			Assert.That(data1 == (null as ExpressionBodiedData)!, Is.False);

			Assert.That(data1 != data2, Is.True);
			Assert.That(data1 != data3, Is.False);
			Assert.That(data2 != data3, Is.True);
		}

		[Test]
		public static void VerifyToString() =>
			Assert.That(new ExpressionBodiedData(6, 1, 2, 3).ToString(), 
				Is.EqualTo("TotalOccurences = 6, ArrowSingleLineOccurences = 1, ArrowMultiLineOccurences = 2, BlockOccurences = 3"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new ExpressionBodiedData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new ExpressionBodiedData(6, 1, 2, 3);
			var data2 = new ExpressionBodiedData(6, 2, 1, 3);
			var data3 = new ExpressionBodiedData(6, 1, 2, 3);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}