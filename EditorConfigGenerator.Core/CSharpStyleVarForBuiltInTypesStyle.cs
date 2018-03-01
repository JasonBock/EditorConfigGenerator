using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace EditorConfigGenerator.Core
{
	public sealed class CSharpStyleVarForBuiltInTypesStyle
		: Style<BooleanStatistics, LocalDeclarationStatementSyntax, CSharpStyleVarForBuiltInTypesStyle>
	{
		public CSharpStyleVarForBuiltInTypesStyle(BooleanStatistics statistics)
			: base(statistics) { }

		public override CSharpStyleVarForBuiltInTypesStyle Update(
			LocalDeclarationStatementSyntax node)
		{
			if(!node.ContainsDiagnostics)
			{
				var variableDeclaration = node.ChildNodes()
					.Single(_ => _.RawKind == (int)SyntaxKind.VariableDeclaration);
				var identifierName = variableDeclaration.ChildNodes()
					.SingleOrDefault(_ => _.RawKind == (int)SyntaxKind.IdentifierName);

				return identifierName != null ?
					new CSharpStyleVarForBuiltInTypesStyle(
						this.Statistics.Update((identifierName as IdentifierNameSyntax).IsVar)) :
					new CSharpStyleVarForBuiltInTypesStyle(
						this.Statistics.Update(false));
			}
			else
			{
				return this;
			}
		}

		public override string Setting => "csharp_style_var_for_built_in_types";
	}
}
