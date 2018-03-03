using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EditorConfigGenerator.Core
{
	internal sealed class StyleWalker
		: CSharpSyntaxWalker
	{
		internal StyleWalker()
			: base(SyntaxWalkerDepth.Trivia) { }

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
