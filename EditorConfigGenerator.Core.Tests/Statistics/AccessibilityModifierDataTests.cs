using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	public static class AccessibilityModifierDataTests
	{
		[Test]
		public static void Create()
		{
			var totalOccurences = 10u;
			var notProvidedOccurences = 2u;
			var providedDefaultOccurences = 5u;
			var providedNotDefaultOccurences = 3u;
			var notProvidedForPublicInterfaceMembersOccurences = 1u;
			var providedForPublicInterfaceMembersOccurences = 5u;

			var data = new AccessibilityModifierData(totalOccurences, notProvidedOccurences, providedDefaultOccurences,
				providedNotDefaultOccurences, notProvidedForPublicInterfaceMembersOccurences, providedForPublicInterfaceMembersOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(notProvidedOccurences), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(providedDefaultOccurences), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(providedNotDefaultOccurences), nameof(data.ProvidedNotDefaultOccurences));
			Assert.That(data.NotProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(notProvidedForPublicInterfaceMembersOccurences), nameof(data.NotProvidedForPublicInterfaceMembersOccurences));
			Assert.That(data.ProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(providedForPublicInterfaceMembersOccurences), nameof(data.ProvidedForPublicInterfaceMembersOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new AccessibilityModifierData();

			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(default(uint)), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(default(uint)), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(default(uint)), nameof(data.ProvidedNotDefaultOccurences));
			Assert.That(data.NotProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(default(uint)), nameof(data.NotProvidedForPublicInterfaceMembersOccurences));
			Assert.That(data.ProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(default(uint)), nameof(data.ProvidedForPublicInterfaceMembersOccurences));
		}

		[Test]
		public static void EvaluateConsistency() =>
			Assert.That(new AccessibilityModifierData(100u, 10u, 70u, 20u, 5u, 50u).Consistency, Is.EqualTo(0.550000012f));

		[Test]
		public static void Add()
		{
			var data1 = new AccessibilityModifierData(10u, 2u, 5u, 3u, 1u, 5u);
			var data2 = new AccessibilityModifierData(100u, 20u, 50u, 30u, 10u, 50u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(110u), nameof(data3.TotalOccurences));
			Assert.That(data3.NotProvidedOccurences, Is.EqualTo(22u), nameof(data3.NotProvidedOccurences));
			Assert.That(data3.ProvidedDefaultOccurences, Is.EqualTo(55u), nameof(data3.ProvidedDefaultOccurences));
			Assert.That(data3.ProvidedNotDefaultOccurences, Is.EqualTo(33u), nameof(data3.ProvidedNotDefaultOccurences));
			Assert.That(data3.NotProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(11u), nameof(data3.NotProvidedForPublicInterfaceMembersOccurences));
			Assert.That(data3.ProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(55u), nameof(data3.ProvidedForPublicInterfaceMembersOccurences));
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
			var data = new AccessibilityModifierData(0u, 0u, 0u, 0u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo(string.Empty), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverProvidedDefaultOverNotProvidedAndInterfaceMembersAreProvided()
		{
			var data = new AccessibilityModifierData(6u, 1u, 2u, 3u, 1u, 2u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverProvidedDefaultOverNotProvidedAndInterfaceMembersAreNotProvided()
		{
			var data = new AccessibilityModifierData(6u, 1u, 2u, 3u, 2u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = for_non_interface_members:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverNotProvidedPlusProvidedDefault()
		{
			var data = new AccessibilityModifierData(7u, 2u, 1u, 4u, 1u, 2u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultNotOverSummationOverProvidedDefaultOverNotProvided()
		{
			var data = new AccessibilityModifierData(9u, 2u, 3u, 4u, 1u, 2u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedNotDefaultOverNotProvidedOverProvidedDefault()
		{
			var data = new AccessibilityModifierData(9u, 3u, 2u, 4u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = omit_if_default:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithProvidedDefaultOverNotProvidedOverProvidedNotDefault()
		{
			var data = new AccessibilityModifierData(6u, 2u, 3u, 1u, 0u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedOverSumOfProvided()
		{
			var data = new AccessibilityModifierData(6u, 4u, 1u, 1u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = never:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedNotOverSumOfProvidedAndProvidedDefaultOverProvidedNotDefault()
		{
			var data = new AccessibilityModifierData(9u, 4u, 3u, 2u, 0u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = always:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithNotProvidedNotOverSumOfProvidedAndProvidedNotDefaultOverProvidedDefault()
		{
			var data = new AccessibilityModifierData(9u, 4u, 2u, 3u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = omit_if_default:error"), nameof(data.GetSetting));
		}

		[TestCase(AccessibilityModifierDataOccurence.NotProvided, false, 1u, 0u, 0u, 0u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedDefault, false, 0u, 1u, 0u, 0u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedNotDefault, false, 0u, 0u, 1u, 0u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.NotProvided, true, 1u, 0u, 0u, 1u, 0u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedDefault, true, 0u, 1u, 0u, 0u, 1u)]
		[TestCase(AccessibilityModifierDataOccurence.ProvidedNotDefault, true, 0u, 0u, 1u, 0u, 1u)]
		public static void Update(AccessibilityModifierDataOccurence occurence, bool isFromPublicInterface, uint expectedNotProvidedOccurences,
			uint expectedProvidedDefaultOccurences, uint expectedProvidedNotDefaultOccurences, 
			uint expectedNotProvidedForPublicInterfaceMembersOccurences, uint expectedProvidedForPublicInterfaceMembersOccurences)
		{
			var data = new AccessibilityModifierData();
			data = data.Update(occurence, isFromPublicInterface);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.NotProvidedOccurences, Is.EqualTo(expectedNotProvidedOccurences), nameof(data.NotProvidedOccurences));
			Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(expectedProvidedDefaultOccurences), nameof(data.ProvidedDefaultOccurences));
			Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(expectedProvidedNotDefaultOccurences), nameof(data.ProvidedNotDefaultOccurences));
			Assert.That(data.NotProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(expectedNotProvidedForPublicInterfaceMembersOccurences), nameof(data.NotProvidedForPublicInterfaceMembersOccurences));
			Assert.That(data.ProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(expectedProvidedForPublicInterfaceMembersOccurences), nameof(data.ProvidedForPublicInterfaceMembersOccurences));
		}

		[Test]
		public static void VerifyEquality()
		{
			var data1 = new AccessibilityModifierData(6, 1, 2, 3, 1, 2);
			var data2 = new AccessibilityModifierData(6, 2, 1, 3, 1, 2);
			var data3 = new AccessibilityModifierData(6, 1, 2, 3, 1, 2);

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
			Assert.That(new AccessibilityModifierData(6, 1, 2, 3, 4, 5).ToString(),
				Is.EqualTo("TotalOccurences = 6, NotProvidedOccurences = 1, ProvidedDefaultOccurences = 2, ProvidedNotDefaultOccurences = 3, NotProvidedForPublicInterfaceMembersOccurences = 4, ProvidedForPublicInterfaceMembersOccurences = 5"));

		[Test]
		public static void VerifyEqualityWithInvalidType() =>
			Assert.That(new AccessibilityModifierData().Equals(new object()), Is.False);

		[Test]
		public static void VerifyHashCodes()
		{
			var data1 = new AccessibilityModifierData(6, 1, 2, 3, 1, 2);
			var data2 = new AccessibilityModifierData(6, 2, 1, 3, 1, 2);
			var data3 = new AccessibilityModifierData(6, 1, 2, 3, 1, 2);

			Assert.That(data1.GetHashCode(), Is.Not.EqualTo(data2.GetHashCode()));
			Assert.That(data1.GetHashCode(), Is.EqualTo(data3.GetHashCode()));
		}
	}
}