using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleVarForBuiltInTypesStyle
		: Style<BooleanData, LocalDeclarationStatementSyntax, CSharpStyleVarForBuiltInTypesStyle>
	{
		public CSharpStyleVarForBuiltInTypesStyle(BooleanData data)
			: base(data) { }

		public override CSharpStyleVarForBuiltInTypesStyle Add(CSharpStyleVarForBuiltInTypesStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleVarForBuiltInTypesStyle(this.Data.Add(style.Data));
		}

		public override CSharpStyleVarForBuiltInTypesStyle Update(
			LocalDeclarationStatementSyntax node)
		{
			if(node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				var variableDeclaration = node.ChildNodes()
					.Single(_ => _.Kind() == SyntaxKind.VariableDeclaration);
				var identifierName = variableDeclaration.ChildNodes()
					.SingleOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName);

				return identifierName != null ?
					new CSharpStyleVarForBuiltInTypesStyle(
						this.Data.Update((identifierName as IdentifierNameSyntax).IsVar)) :
					new CSharpStyleVarForBuiltInTypesStyle(
						this.Data.Update(false));
			}
			else
			{
				return this;
			}
		}

		public override string Setting => "csharp_style_var_for_built_in_types";
	}
}