﻿using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class CSharpSpaceBetweenMethodCallParameterListParenthesesStyleTests
{
	[Test]
	public static void CreateWithCustomSeverity()
	{
		const Severity suggestion = Severity.Suggestion;
		var data = new BooleanData();
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(data, suggestion);
		Assert.That(style.Severity, Is.EqualTo(suggestion), nameof(style.Data));
	}

	[Test]
	public static void CreateWithNoData()
	{
		var data = new BooleanData();
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
	}

	[Test]
	public static void CreateWithMoreFalseData()
	{
		var data = new BooleanData(1u, 0u, 1u);
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.Setting} = false:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void CreateWithMoreTrueData()
	{
		var data = new BooleanData(1u, 1u, 0u);
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(data);
		Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
		Assert.That(style.GetSetting(), Is.EqualTo(
			$"{CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.Setting} = true:{style.Severity.GetDescription()}"), nameof(style.GetSetting));
	}

	[Test]
	public static void Add()
	{
		var style1 = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData(3u, 1u, 2u));
		var style2 = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData(30u, 10u, 20u));
		var style3 = style1.Add(style2);

		var data = style3.Data;
		Assert.That(data.TotalOccurences, Is.EqualTo(33u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(11u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(22u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void AddWithNull()
	{
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData());
		Assert.That(() => style.Add(null!), Throws.TypeOf<ArgumentNullException>());
	}

	[Test]
	public static void UpdateWithNull()
	{
		var data = new BooleanData(default, default, default);
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(data);

		Assert.That(() => style.Update(null!), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
	}

	[Test]
	public static void UpdateWithSpacesInArgumentList()
	{
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData(default, default, default));

		var statement = (ArgumentListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public void Bar(int x) 
	{
		Bar( 3 );
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ArgumentList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
	}

	[Test]
	public static void UpdateWithNoSpacesInParameterList()
	{
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData(default, default, default));

		var statement = (ArgumentListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public void Bar(int x) 
	{
		Bar(3);
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ArgumentList);
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
		var style = new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData(default, default, default));

		var statement = (ArgumentListSyntax)SyntaxFactory.ParseCompilationUnit(
 @"public class Foo
{
	public void Bar(int x) 
	{
		Bar( 3=> );
	}
}", options: Shared.ParseOptions).DescendantNodes().Single(_ => _.Kind() == SyntaxKind.ArgumentList);
		var newStyle = style.Update(statement);

		var data = newStyle.Data;
		Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
		Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
		Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
		Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
	}
}