using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using Spackle;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class BooleanDataTests
	{
		[Test]
		public static void Create()
		{
			var generator = new RandomObjectGenerator();
			var totalOccurences = generator.Generate<uint>();
			var trueOccurences = generator.Generate<uint>();
			var falseOccurences = generator.Generate<uint>();

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
	}
}
