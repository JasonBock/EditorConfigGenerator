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
		public static void GetSettingWithProviderNotDefaultOverProviderDefaultOverNotProvided()
		{
			var data = new AccessibilityModifierData(6u, 1u, 2u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}
	}
}