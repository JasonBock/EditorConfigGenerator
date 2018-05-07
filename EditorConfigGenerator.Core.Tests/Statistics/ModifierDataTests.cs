using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class ModifierDataTests
	{
		[Test]
		public static void Create()
		{
			var data = new ModifierData();
			Assert.That(data.VisibilityModifiers.Count, Is.EqualTo(4));
			Assert.That(data.OtherModifiers.Count, Is.EqualTo(11));

			Shared.VerifyModifiers(data.VisibilityModifiers);
			Shared.VerifyModifiers(data.OtherModifiers);
		}

		private static void TestUpdate(string keyword)
		{
			var data = new ModifierData();
			var newData = data.Update(new[] { keyword }.ToImmutableList());

			var pair = new KeyValuePair<string, (uint weight, uint frequency)>(keyword, (1u, 1u));
			Shared.VerifyModifiers(newData.VisibilityModifiers, pair);
			Shared.VerifyModifiers(newData.OtherModifiers, pair);
		}

		[Test]
		public static void UpdateWithPublic() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText);

		[Test]
		public static void UpdateWithPrivate() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.PrivateKeyword).ValueText);

		[Test]
		public static void UpdateWithProtected() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword).ValueText);

		[Test]
		public static void UpdateWithInternal() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.InternalKeyword).ValueText);

		[Test]
		public static void UpdateWithStatic() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText);

		[Test]
		public static void UpdateWithExtern() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.ExternKeyword).ValueText);

		[Test]
		public static void UpdateWithNew() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.NewKeyword).ValueText);

		[Test]
		public static void UpdateWithVirtual() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.VirtualKeyword).ValueText);

		[Test]
		public static void UpdateWithAbstract() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.AbstractKeyword).ValueText);

		[Test]
		public static void UpdateWithSealed() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.SealedKeyword).ValueText);

		[Test]
		public static void UpdateWithOverride() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.OverrideKeyword).ValueText);

		[Test]
		public static void UpdateWithReadOnly() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).ValueText);

		[Test]
		public static void UpdateWithUnsafe() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.UnsafeKeyword).ValueText);

		[Test]
		public static void UpdateWithVolatile() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.VolatileKeyword).ValueText);

		[Test]
		public static void UpdateWithAsync() =>
			ModifierDataTests.TestUpdate(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).ValueText);

		[Test]
		public static void Add()
		{
			var data = new ModifierData();
			data = data.Update(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText }.ToImmutableList());
			data = data.Update(new[] { SyntaxFactory.Token(SyntaxKind.AsyncKeyword).ValueText }.ToImmutableList());

			var newData = new ModifierData().Add(data);

			Shared.VerifyModifiers(newData.VisibilityModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText, (1u, 1u)));
			Shared.VerifyModifiers(newData.OtherModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.AsyncKeyword).ValueText, (1u, 1u)));
		}
	}
}
