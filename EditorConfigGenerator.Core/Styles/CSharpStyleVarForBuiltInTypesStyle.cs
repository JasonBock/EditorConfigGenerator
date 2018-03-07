using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleVarForBuiltInTypesStyle
		: SeverityStyle<BooleanData, LocalDeclarationStatementSyntax, CSharpStyleVarForBuiltInTypesStyle>
	{
		public CSharpStyleVarForBuiltInTypesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleVarForBuiltInTypesStyle Add(CSharpStyleVarForBuiltInTypesStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleVarForBuiltInTypesStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_style_var_for_built_in_types = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleVarForBuiltInTypesStyle Update(
			LocalDeclarationStatementSyntax node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				if(node.DescendantNodes()
					.Any(_ => _.Kind() == SyntaxKind.StringLiteralExpression || 
						_.Kind() == SyntaxKind.NumericLiteralExpression))
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
					return new CSharpStyleVarForBuiltInTypesStyle(this.Data, this.Severity);
				}
			}
			else
			{
				return new CSharpStyleVarForBuiltInTypesStyle(this.Data, this.Severity);
			}
		}
	}
}