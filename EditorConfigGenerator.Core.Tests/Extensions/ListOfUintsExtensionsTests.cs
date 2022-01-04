using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions;

public static class ListOfUintsExtensionsTests
{
	[Test]
	public static void GetConsistencyWhenValuesAreNull() =>
		Assert.That(() => (null as List<uint>)!.GetConsistency(0), Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void GetConsistencyWhenTotalIsZero() =>
		Assert.That(new List<uint>().GetConsistency(0), Is.EqualTo(0f));

	[Test]
	public static void GetConsistencyWithOnlyOneValue() =>
		Assert.That(new List<uint> { 1u }.GetConsistency(1), Is.EqualTo(0f));

	[Test]
	public static void GetConsistency() =>
		Assert.That(new List<uint> { 25u, 25u, 3u, 4u }.GetConsistency(57), Is.EqualTo(0.251461983f));
}