using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
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
			Assert.That(data.VisibilityModifiers[SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.VisibilityModifiers[SyntaxFactory.Token(SyntaxKind.PrivateKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.VisibilityModifiers[SyntaxFactory.Token(SyntaxKind.ProtectedKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.VisibilityModifiers[SyntaxFactory.Token(SyntaxKind.InternalKeyword).ValueText], Is.EqualTo((0u, 0u)));

			Assert.That(data.OtherModifiers.Count, Is.EqualTo(11));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.ExternKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.NewKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.VirtualKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.AbstractKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.SealedKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.OverrideKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.UnsafeKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.VolatileKeyword).ValueText], Is.EqualTo((0u, 0u)));
			Assert.That(data.OtherModifiers[SyntaxFactory.Token(SyntaxKind.AsyncKeyword).ValueText], Is.EqualTo((0u, 0u)));
		}

		private static void TestUpdate(string keyword)
		{
			void IterateModifiers(ImmutableDictionary<string, (uint weight, uint keyword)> modifiers)
			{
				foreach (var modifier in modifiers)
				{
					if (modifier.Key == keyword)
					{
						Assert.That(modifier.Value, Is.EqualTo((1u, 1u)), keyword);
					}
					else
					{
						Assert.That(modifier.Value, Is.EqualTo((0u, 0u)), modifier.Key);
					}
				}
			}

			var data = new ModifierData();
			var newData = data.Update(new[] { keyword }.ToImmutableList());

			IterateModifiers(newData.VisibilityModifiers);
			IterateModifiers(newData.OtherModifiers);
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
	}
}
