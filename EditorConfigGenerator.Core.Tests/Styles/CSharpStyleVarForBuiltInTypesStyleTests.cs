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
			var data = new BooleanData(default, default, default);
			var style = new CSharpStyleVarForBuiltInTypesStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.Setting, Is.EqualTo("csharp_style_var_for_built_in_types"), nameof(style.Setting));
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
			style = style.Update(statement);

			var data = style.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(1u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(0u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithTypeDeclaration()
		{
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("int x = 0;") as LocalDeclarationStatementSyntax;
			style = style.Update(statement);

			var data = style.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(0u), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(1u), nameof(data.FalseOccurences));
		}

		[Test]
		public static void UpdateWithDiagnostics()
		{
			var style = new CSharpStyleVarForBuiltInTypesStyle(new BooleanData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("int x = 0") as LocalDeclarationStatementSyntax;
			style = style.Update(statement);

			var data = style.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.TrueOccurences, Is.EqualTo(default(uint)), nameof(data.TrueOccurences));
			Assert.That(data.FalseOccurences, Is.EqualTo(default(uint)), nameof(data.FalseOccurences));
		}
	}
}
