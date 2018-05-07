using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class CSharpPreferredModifierOrderStyleTests
	{
		[Test]
		public static void Create()
		{
			var data = new ModifierData();
			var style = new CSharpPreferredModifierOrderStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithModifier()
		{
			var data = new ModifierData();
			data = data.Update(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText }.ToImmutableList());
			var style = new CSharpPreferredModifierOrderStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("csharp_preferred_modifier_order = public"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpPreferredModifierOrderStyle(
				new ModifierData().Update(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText }.ToImmutableList()));
			var style2 = new CSharpPreferredModifierOrderStyle(
				new ModifierData().Update(new[] { SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText }.ToImmutableList()));
			var style3 = style1.Add(style2);

			var data = style3.Data;

			Assert.That(data.TotalOccurences, Is.EqualTo(2u), nameof(data.TotalOccurences));
			Shared.VerifyModifiers(data.VisibilityModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText, (1u, 1u)));
			Shared.VerifyModifiers(data.OtherModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText, (1u, 1u)));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpPreferredModifierOrderStyle(new ModifierData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new ModifierData();
			var style = new CSharpPreferredModifierOrderStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithVisibilityAndOtherModifierSpecified()
		{
			var style = new CSharpPreferredModifierOrderStyle(new ModifierData());

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public static void Bar() { }
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(2u), nameof(data.TotalOccurences));

			Shared.VerifyModifiers(data.VisibilityModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText, (2u, 1u)));
			Shared.VerifyModifiers(data.OtherModifiers,
				new KeyValuePair<string, (uint weight, uint frequency)>(
					SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText, (1u, 1u)));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new CSharpPreferredModifierOrderStyle(new ModifierData());

			var statement = SyntaxFactory.ParseCompilationUnit(
@"public class Foo
{
	public static void Bar()=> { }
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Shared.VerifyModifiers(data.VisibilityModifiers);
			Shared.VerifyModifiers(data.OtherModifiers);
		}
	}
}
