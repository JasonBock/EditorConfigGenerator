using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using Spackle;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class ParenthesesSpaceDataTests
	{
		[Test]
		public static void Create()
		{
			var generator = new RandomObjectGenerator();
			var totalOccurences = generator.Generate<uint>();
			var controlFlowNoSpaceOccurences = generator.Generate<uint>();
			var controlFlowSpaceOccurences = generator.Generate<uint>();
			var expressionsNoSpaceOccurences = generator.Generate<uint>();
			var expressionsSpaceOccurences = generator.Generate<uint>();
			var typeCastsNoSpaceOccurences = generator.Generate<uint>();
			var typeCastsSpaceOccurences = generator.Generate<uint>();

			var data = new ParenthesesSpaceData(totalOccurences, controlFlowNoSpaceOccurences, controlFlowSpaceOccurences,
				expressionsNoSpaceOccurences, expressionsSpaceOccurences,
				typeCastsNoSpaceOccurences, typeCastsSpaceOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(controlFlowNoSpaceOccurences), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(controlFlowSpaceOccurences), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(expressionsNoSpaceOccurences), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(expressionsSpaceOccurences), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(typeCastsNoSpaceOccurences), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(typeCastsSpaceOccurences), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new ParenthesesSpaceData();
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(default(uint)), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void EvaluateConsistency() =>
			Assert.That(new ParenthesesSpaceData(100u, 20u, 5u, 40u, 10u, 20u, 5u).Consistency, Is.EqualTo(0.600000024f));

		[Test]
		public static void Add()
		{
			var data1 = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			var data2 = new ParenthesesSpaceData(210u, 10u, 20u, 30u, 40u, 50u, 60u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(231u), nameof(data3.TotalOccurences));
			Assert.That(data3.ControlFlowNoSpaceOccurences, Is.EqualTo(11u), nameof(data3.ControlFlowNoSpaceOccurences));
			Assert.That(data3.ControlFlowSpaceOccurences, Is.EqualTo(22u), nameof(data3.ControlFlowSpaceOccurences));
			Assert.That(data3.ExpressionsNoSpaceOccurences, Is.EqualTo(33u), nameof(data3.ExpressionsNoSpaceOccurences));
			Assert.That(data3.ExpressionsSpaceOccurences, Is.EqualTo(44u), nameof(data3.ExpressionsSpaceOccurences));
			Assert.That(data3.TypeCastsNoSpaceOccurences, Is.EqualTo(55u), nameof(data3.TypeCastsNoSpaceOccurences));
			Assert.That(data3.TypeCastsSpaceOccurences, Is.EqualTo(66u), nameof(data3.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new ParenthesesSpaceData();
			Assert.That(() => data.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithControlFlowTrue()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateControlFlow(true);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(3u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(3u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(5u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithControlFlowFalse()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateControlFlow(false);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(3u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(5u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithExpressionTrue()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateExpression(true);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(3u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(5u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(5u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithExpressionFalse()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateExpression(false);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(5u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithTypeCastTrue()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateTypeCast(true);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(3u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(5u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(7u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void UpdateWithTypeCastFalse()
		{
			var data = new ParenthesesSpaceData(21u, 1u, 2u, 3u, 4u, 5u, 6u);
			data = data.UpdateTypeCast(false);

			Assert.That(data.TotalOccurences, Is.EqualTo(22u), nameof(data.TotalOccurences));
			Assert.That(data.ControlFlowNoSpaceOccurences, Is.EqualTo(1u), nameof(data.ControlFlowNoSpaceOccurences));
			Assert.That(data.ControlFlowSpaceOccurences, Is.EqualTo(2u), nameof(data.ControlFlowSpaceOccurences));
			Assert.That(data.ExpressionsNoSpaceOccurences, Is.EqualTo(3u), nameof(data.ExpressionsNoSpaceOccurences));
			Assert.That(data.ExpressionsSpaceOccurences, Is.EqualTo(4u), nameof(data.ExpressionsSpaceOccurences));
			Assert.That(data.TypeCastsNoSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsNoSpaceOccurences));
			Assert.That(data.TypeCastsSpaceOccurences, Is.EqualTo(6u), nameof(data.TypeCastsSpaceOccurences));
		}

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new ParenthesesSpaceData(21, 1, 2, 3, 4, 5, 6);
			var data2 = new ParenthesesSpaceData(21, 6, 5, 4, 3, 2, 1);
			var data3 = new ParenthesesSpaceData(21, 1, 2, 3, 4, 5, 6);

			Assert.That(data1, Is.Not.EqualTo(data2));
			Assert.That(data1, Is.EqualTo(data3));
			Assert.That(data2, Is.Not.EqualTo(data3));

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.That(data1 == data1, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.That(data1 == data2, Is.False);
			Assert.That(data1 == data3, Is.True);
			Assert.That(data2 == data3, Is.False);
			Assert.That((null as ParenthesesSpaceData)! == data1, Is.False);
			Assert.That(data1 == (null as ParenthesesSpaceData)!, Is.False);

			Assert.That(data1 != data2, Is.True);
			Assert.That(data1 != data3, Is.False);
			Assert.That(data2 != data3, Is.True);
		}

		[Test]
		public static void VerifyToString() =>
			Assert.That(new ParenthesesSpaceData(21, 1, 2, 3, 4, 5, 6).ToString(), 
				Is.EqualTo("TotalOccurences = 21, ControlFlowNoSpaceOccurences = 1, ControlFlowSpaceOccurences = 2, ExpressionsNoSpaceOccurences = 3, ExpressionsSpaceOccurences = 4, TypeCastsNoSpaceOccurences = 5, TypeCastsSpaceOccurences = 6"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new ParenthesesSpaceData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new ParenthesesSpaceData(21, 1, 2, 3, 4, 5, 6);
			var data2 = new ParenthesesSpaceData(21, 6, 5, 4, 3, 2, 1);
			var data3 = new ParenthesesSpaceData(21, 1, 2, 3, 4, 5, 6);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}