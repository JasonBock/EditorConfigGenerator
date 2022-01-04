using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class CSharpPreserveSingleLineBlocksStyleTests
{
	[Test]
	public static void CreateWithCustomSeverity()
	{
		const Severity suggestion = Severity.Suggestion;
		var data = new BooleanData();
		var style = new CSharpPreserveSingleLineBlocksStyle(data, suggestion);
		Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Severity));
	}

	[Test]
	public static void CreateWithNoData()
	{
		var data = new BooleanData();
		var style = new CSharpPreserveSingleLineBlocksStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
	}

	[Test]
	public static void CreateWithMoreFalseData()
	{
		var data = new BooleanData(1u, 0u, 1u);
		var style = new CSharpPreserveSingleLineBlocksStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{CSharpPreserveSingleLineBlocksStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void CreateWithMoreTrueData()
	{
		var data = new BooleanData(1u, 1u, 0u);
		var style = new CSharpPreserveSingleLineBlocksStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{CSharpPreserveSingleLineBlocksStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void Add()
	{
		var style1 = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(3u, 1u, 2u));
		var style2 = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(30u, 10u, 20u));
		var style3 = style1.Add(style2);

		var data = style3.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void AddWithNull()
	{
		var style = new CSharpPreserveSingleLineBlocksStyle(new BooleanData());
		Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public static void UpdateWithNull()
	{
		var data = new BooleanData(default, default, default);
		var style = new CSharpPreserveSingleLineBlocksStyle(data);

		Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
	}

	[Test]
	public static void UpdateWithNoLinesInBlock()
	{
		var style = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(default, default, default));

		var statement = (AccessorListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public int X { get; set; }
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.AccessorList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void UpdateWithNoTriviaInOpenBraceOfBlock()
	{
		var style = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(default, default, default));

		var statement = (AccessorListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public int X {get; set; }
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.AccessorList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void UpdateWithLinesInBlock()
	{
		var style = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(default, default, default));

		var statement = (AccessorListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public int X 
	{ 
		get; set; 
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.AccessorList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void UpdateWithDiagnostics()
	{
		var style = new CSharpPreserveSingleLineBlocksStyle(new BooleanData(default, default, default));

		var statement = (AccessorListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public int X {=> get; set; }
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.AccessorList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
	}
}