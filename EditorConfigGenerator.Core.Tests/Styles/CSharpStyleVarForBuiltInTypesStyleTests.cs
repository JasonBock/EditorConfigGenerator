using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class CSharpStyleVarForBuiltInTypesStyleTests
	{
		[Test]
		public static void Create()
		{
			var data = new BooleanData();
			var style = new CSharpStyleVarForBuiltInTypesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.Setting, Is.EqualTo("csharp_style_var_for_built_in_types"), nameof(style.Setting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(1u, 2u, 3u));
			var style2 = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(22u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(33u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new BooleanData(default, default, default);
			var style = new CSharpStyleVarForBuiltInTypesStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithVarDeclaration()
		{
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("var x = 0;") as LocalDeclarationStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithTypeDeclaration()
		{
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("int x = 0;") as LocalDeclarationStatementSyntax;
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
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("int x = 0") as LocalDeclarationStatementSyntax;
			var newStyle = style.Update(statement);

			Assert.That(newStyle, Is.SameAs(style), nameof(newStyle));
		}
	}
}
