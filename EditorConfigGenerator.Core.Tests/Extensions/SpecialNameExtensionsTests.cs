using Microsoft.CodeAnalysis;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.SpecialNameExtensions;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	[TestFixture]
	public static class SpecialNameExtensionsTests
	{
		[Test]
		public static void IsBooleanPredefinedType() => 
			Assert.That(SpecialType.System_Boolean.IsPredefinedType(), Is.True);

		[Test]
		public static void IsBytePredefinedType() =>
			Assert.That(SpecialType.System_Byte.IsPredefinedType(), Is.True);

		[Test]
		public static void IsSBytePredefinedType() =>
			Assert.That(SpecialType.System_SByte.IsPredefinedType(), Is.True);

		[Test]
		public static void IsCharPredefinedType() =>
			Assert.That(SpecialType.System_Char.IsPredefinedType(), Is.True);

		[Test]
		public static void IsDecimalPredefinedType() =>
			Assert.That(SpecialType.System_Decimal.IsPredefinedType(), Is.True);

		[Test]
		public static void IsDoublePredefinedType() =>
			Assert.That(SpecialType.System_Double.IsPredefinedType(), Is.True);

		[Test]
		public static void IsSinglePredefinedType() =>
			Assert.That(SpecialType.System_Single.IsPredefinedType(), Is.True);

		[Test]
		public static void IsInt32PredefinedType() =>
			Assert.That(SpecialType.System_Int32.IsPredefinedType(), Is.True);

		[Test]
		public static void IsUInt32PredefinedType() =>
			Assert.That(SpecialType.System_UInt32.IsPredefinedType(), Is.True);

		[Test]
		public static void IsInt64PredefinedType() =>
			Assert.That(SpecialType.System_Int64.IsPredefinedType(), Is.True);

		[Test]
		public static void IsUInt64PredefinedType() =>
			Assert.That(SpecialType.System_UInt64.IsPredefinedType(), Is.True);

		[Test]
		public static void IsObjectPredefinedType() =>
			Assert.That(SpecialType.System_Object.IsPredefinedType(), Is.True);

		[Test]
		public static void IsInt16PredefinedType() =>
			Assert.That(SpecialType.System_Int16.IsPredefinedType(), Is.True);

		[Test]
		public static void IsUInt16PredefinedType() =>
			Assert.That(SpecialType.System_UInt16.IsPredefinedType(), Is.True);

		[Test]
		public static void IsStringPredefinedType() =>
			Assert.That(SpecialType.System_String.IsPredefinedType(), Is.True);

		[Test]
		public static void IsNotPredefinedType() =>
			Assert.That(SpecialType.System_ArgIterator.IsPredefinedType(), Is.False);
	}
}
