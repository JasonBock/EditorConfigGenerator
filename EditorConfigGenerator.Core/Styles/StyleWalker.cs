using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;

namespace EditorConfigGenerator.Core.Styles
{
	internal sealed class StyleWalker
		: CSharpSyntaxWalker
	{
		internal StyleWalker()
			: base(SyntaxWalkerDepth.Trivia) { }

		public StyleWalker Add(StyleWalker walker)
		{
			if (walker == null) { throw new ArgumentNullException(nameof(walker)); }
			return new StyleWalker
			{
				CSharpStyleExpressionBodiedMethodsStyle =
					this.CSharpStyleExpressionBodiedMethodsStyle.Add(walker.CSharpStyleExpressionBodiedMethodsStyle),
				CSharpStyleVarForBuiltInTypesStyle =
					this.CSharpStyleVarForBuiltInTypesStyle.Add(walker.CSharpStyleVarForBuiltInTypesStyle),
				IndentStyleStyle = 
					this.IndentStyleStyle.Add(walker.IndentStyleStyle)
			};
		}

		public string GenerateConfiguration()
		{
			var builder = new StringBuilder();
			builder.AppendLine("[*.cs]");
			StyleWalker.AppendSetting(
				this.CSharpStyleExpressionBodiedMethodsStyle.GetSetting(), builder);
			StyleWalker.AppendSetting(
				this.CSharpStyleVarForBuiltInTypesStyle.GetSetting(), builder);
			StyleWalker.AppendSetting(
				this.IndentStyleStyle.GetSetting(), builder);
			return builder.ToString();
		}

		private static void AppendSetting(string setting, StringBuilder builder)
		{
			if (!string.IsNullOrWhiteSpace(setting)) { builder.AppendLine(setting); }
		}

		public override void Visit(SyntaxNode node)
		{
			this.IndentStyleStyle =
				this.IndentStyleStyle.Update(node);
			base.Visit(node);
		}

		public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			this.CSharpStyleVarForBuiltInTypesStyle =
				this.CSharpStyleVarForBuiltInTypesStyle.Update(node);
			base.VisitLocalDeclarationStatement(node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			this.CSharpStyleExpressionBodiedMethodsStyle =
				this.CSharpStyleExpressionBodiedMethodsStyle.Update(node);
			base.VisitMethodDeclaration(node);
		}

		public CSharpStyleExpressionBodiedMethodsStyle CSharpStyleExpressionBodiedMethodsStyle { get; private set; } =
			new CSharpStyleExpressionBodiedMethodsStyle(new BooleanData());
		public CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; private set; } =
			new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
		public IndentStyleStyle IndentStyleStyle { get; private set; } =
			new IndentStyleStyle(new TabSpaceData());
	}
}
