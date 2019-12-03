using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using Spackle;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	public static class AccessibilityModifierDataTests
	{
		[Test]
		public static void Create()
		{
			var generator = new RandomObjectGenerator();
			var totalOccurences = generator.Generate<uint>();
			var notProvidedOccurences = generator.Generate<uint>();
			var providedDefaultOccurences = generator.Generate<uint>();
			var providedNotDefaultOccurences = generator.Generate<uint>();

			var data = new AccessibilityModifierData(totalOccurences, notProvidedOccurences, providedDefaultOccurences,
				providedNotDefaultOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(notProvidedOccurences), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(providedDefaultOccurences), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(providedNotDefaultOccurences), nameof(data.ProvidedNotDefaultOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new AccessibilityModifierData();

			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(default(uint)), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(default(uint)), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(default(uint)), nameof(data.ProvidedNotDefaultOccurences));
		}

		[Test]
		public static void Add()
		{
			var data1 = new AccessibilityModifierData(10u, 1u, 2u, 3u);
			var data2 = new AccessibilityModifierData(100u, 10u, 20u, 30u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(110u), nameof(data3.TotalOccurences));
			Assert.That(data3.NotProvidedOccurences, Is.EqualTo(11u), nameof(data3.NotProvidedOccurences));
			Assert.That(data3.ProvidedDefaultOccurences, Is.EqualTo(22u), nameof(data3.ProvidedDefaultOccurences));
			Assert.That(data3.ProvidedNotDefaultOccurences, Is.EqualTo(33u), nameof(data3.ProvidedNotDefaultOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new AccessibilityModifierData();
			Assert.That(() => data.Add(null!), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void GetSettingWithNoOccurrences()
		{
			var data = new AccessibilityModifierData(0u, 0u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo(string.Empty), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverProvidedDefaultOverNotProvided()
		{
			var data = new AccessibilityModifierData(6u, 1u, 2u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverNotProvidedOverProvidedDefault()
		{
			var data = new AccessibilityModifierData(6u, 2u, 1u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = omit_if_default:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedDefaultOverNotProvidedOverProvidedNotDefault()
		{
			var data = new AccessibilityModifierData(6u, 2u, 3u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedOverSumOfProvided()
		{
			var data = new AccessibilityModifierData(6u, 4u, 1u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = never:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedNotOverSumOfProvidedAndProvidedDefaultOverProvidedNotDefault()
		{
			var data = new AccessibilityModifierData(9u, 4u, 3u, 2u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedNotOverSumOfProvidedAndProvidedNotDefaultOverProvidedDefault()
		{
			var data = new AccessibilityModifierData(9u, 4u, 2u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = omit_if_default:error"), nameof(data.GetSetting));
		}

		[TestCase(AccessibilityModifierDataOccurence.NotProvided, 1u, 0u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedDefault, 0u, 1u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedNotDefault, 0u, 0u, 1u)]
		public static void Update(AccessibilityModifierDataOccurence occurence, uint expectedNotProvidedOccurences,
			uint expectedProvidedDefaultOccurences, uint expectedProvidedNotDefaultOccurences)
		{
			var data = new AccessibilityModifierData();
			data = data.Update(occurence);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(expectedNotProvidedOccurences), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(expectedProvidedDefaultOccurences), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(expectedProvidedNotDefaultOccurences), nameof(data.ProvidedNotDefaultOccurences));
		}

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new AccessibilityModifierData(6, 1, 2, 3);
			var data2 = new AccessibilityModifierData(6, 2, 1, 3);
			var data3 = new AccessibilityModifierData(6, 1, 2, 3);

			Assert.That(data1, Is.Not.EqualTo(data2));
			Assert.That(data1, Is.EqualTo(data3));
			Assert.That(data2, Is.Not.EqualTo(data3));

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.That(data1 == data1, Is.True);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.That(data1 == data2, Is.False);
			Assert.That(data1 == data3, Is.True);
			Assert.That(data2 == data3, Is.False);
			Assert.That((null as AccessibilityModifierData)! == data1, Is.False);
			Assert.That(data1 == (null as AccessibilityModifierData)!, Is.False);

			Assert.That(data1 != data2, Is.True);
			Assert.That(data1 != data3, Is.False);
			Assert.That(data2 != data3, Is.True);
		}

		[Test]
		public static void VerifyToString() =>
			Assert.That(new AccessibilityModifierData(6, 1, 2, 3).ToString(),
				Is.EqualTo("TotalOccurences = 6, NotProvidedOccurences = 1, ProvidedDefaultOccurences = 2, ProvidedNotDefaultOccurences = 3"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new AccessibilityModifierData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new AccessibilityModifierData(6, 1, 2, 3);
			var data2 = new AccessibilityModifierData(6, 2, 1, 3);
			var data3 = new AccessibilityModifierData(6, 1, 2, 3);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}