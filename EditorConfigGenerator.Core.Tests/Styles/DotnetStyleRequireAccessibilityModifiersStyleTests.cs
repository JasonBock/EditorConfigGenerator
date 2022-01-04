using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class DotnetStyleRequireAccessibilityModifiersStyleTests
{
	[Test]
	public static void CreateWithCustomSeverity()
	{
		const Severity suggestion = Severity.Suggestion;
		var data = new AccessibilityModifierData();
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(data, suggestion);
		Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
	}

	[Test]
	public static void CreateWithNoData()
	{
		var data = new AccessibilityModifierData();
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
	}

	[Test]
	public static void GetSetting()
	{
		var data = new AccessibilityModifierData(6u, 2u, 1u, 3u, 1u, 2u);
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{DotnetStyleRequireAccessibilityModifiersStyle.Setting} = always:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void Add()
	{
		var style1 = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(6u, 3u, 2u, 1u, 5u, 4u));
		var style2 = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(60u, 30u, 20u, 10u, 50u, 40u));
		var style3 = style1.Add(style2);

		var data = style3.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(66u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(33u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(22u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(11u), nameof(data.ProvidedNotDefaultOccurences));
		Assert.That(data.NotProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(55u), nameof(data.NotProvidedForPublicInterfaceMembersOccurences));
		Assert.That(data.ProvidedForPublicInterfaceMembersOccurences, Is.EqualTo(44u), nameof(data.ProvidedForPublicInterfaceMembersOccurences));
	}

	[Test]
	public static void AddWithNull()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData());
		Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public static void UpdateWithNull()
	{
		var data = new AccessibilityModifierData(default, default, default, default, default, default);
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(data);

		Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
	}

	[Test]
	public static void UpdateWithPublicClass()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInternalClass()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"internal class Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithClassNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"class Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicStruct()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public struct Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInternalStruct()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"internal struct Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithStructNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"struct Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicInterface()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public interface Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInternalInterface()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"internal interface Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInterfaceNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"interface Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicDelegate()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public delegate void Foo();", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInternalDelegate()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"internal delegate void Foo();", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithDelegateNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"delegate void Foo();", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPublicClass()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public class Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPrivateClass()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private class Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedClassNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	class Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPublicStruct()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public struct Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPrivateStruct()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private struct Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedStructNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	struct Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPublicInterface()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public interface Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPrivateInterface()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private interface Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedInterfaceNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	interface Nested { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.ValueText == "Nested");
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPublicDelegate()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public delegate void Nested();
}", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedPrivateDelegate()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private delegate void Nested();
}", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithNestedDelegateNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	delegate void Nested();
}", options: Shared.ParseOptions).DescendantNodes().OfType<DelegateDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicEnum()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public enum Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<EnumDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithInternalEnum()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"internal enum Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<EnumDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithEnumNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"enum Foo { }", options: Shared.ParseOptions).DescendantNodes().OfType<EnumDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicConstructor()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public Foo() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPrivateConstructor()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private Foo() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithConstructorNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	Foo() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicMethod()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public void Bar() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPrivateMethod()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private void Bar() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithMethodNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	void Bar() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicProperty()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public int Bar { get; }
}", options: Shared.ParseOptions).DescendantNodes().OfType<PropertyDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPrivateProperty()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private int Bar { get; }
}", options: Shared.ParseOptions).DescendantNodes().OfType<PropertyDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPropertyNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	int Bar { get; }
}", options: Shared.ParseOptions).DescendantNodes().OfType<PropertyDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicEvent()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public event EventHandler MyEvent;
}", options: Shared.ParseOptions).DescendantNodes().OfType<EventFieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPrivateEvent()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private event EventHandler MyEvent;
}", options: Shared.ParseOptions).DescendantNodes().OfType<EventFieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithEventNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	event EventHandler MyEvent;
}", options: Shared.ParseOptions).DescendantNodes().OfType<EventFieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPublicField()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	public int foo;
}", options: Shared.ParseOptions).DescendantNodes().OfType<FieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithPrivateField()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	private int foo;
}", options: Shared.ParseOptions).DescendantNodes().OfType<FieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithFieldNoModifier()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	int foo;
}", options: Shared.ParseOptions).DescendantNodes().OfType<FieldDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(1u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithProtectedInternalMethod()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"public class Foo 
{ 
	protected internal void Foo() { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(1u), nameof(data.ProvidedNotDefaultOccurences));
	}

	[Test]
	public static void UpdateWithDiagnostics()
	{
		var style = new DotnetStyleRequireAccessibilityModifiersStyle(new AccessibilityModifierData(default, default, default, default, default, default));

		var statement = SyntaxFactory.ParseCompilationUnit(
 @"class Foo
{
	void Bar( { }
}", options: Shared.ParseOptions).DescendantNodes().OfType<MethodDeclarationSyntax>().First();
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
		Assert.That(data.NotProvidedOccurences, Is.EqualTo(0u), nameof(data.NotProvidedOccurences));
		Assert.That(data.ProvidedDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedDefaultOccurences));
		Assert.That(data.ProvidedNotDefaultOccurences, Is.EqualTo(0u), nameof(data.ProvidedNotDefaultOccurences));
	}
}