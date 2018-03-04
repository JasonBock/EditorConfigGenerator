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
				CSharpStyleVarForBuiltInTypesStyle =
					this.CSharpStyleVarForBuiltInTypesStyle.Add(walker.CSharpStyleVarForBuiltInTypesStyle)
			};
		}

		public string GenerateConfiguration()
		{
			var builder = new StringBuilder();
			builder.AppendLine("[*.cs]");
			StyleWalker.AppendSetting(
				this.CSharpStyleVarForBuiltInTypesStyle.GetSetting(), builder);
			return builder.ToString();
		}

		private static void AppendSetting(string setting, StringBuilder builder)
		{
			if (!string.IsNullOrWhiteSpace(setting)) { builder.AppendLine(setting); }
		}

		public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			this.CSharpStyleVarForBuiltInTypesStyle =
				this.CSharpStyleVarForBuiltInTypesStyle.Update(node);
			base.VisitLocalDeclarationStatement(node);
		}

		public CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; private set; } =
			new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
	}
}
