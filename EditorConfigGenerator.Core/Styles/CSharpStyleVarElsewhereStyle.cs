using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleVarElsewhereStyle
		: SeverityStyle<BooleanData, LocalDeclarationStatementSyntax, CSharpStyleVarElsewhereStyle>
	{
		public CSharpStyleVarElsewhereStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleVarElsewhereStyle Add(CSharpStyleVarElsewhereStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleVarElsewhereStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_style_var_elsewhere = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleVarElsewhereStyle Update(LocalDeclarationStatementSyntax node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				if(node.DescendantNodes()
					.Any(_ => _.Kind() != SyntaxKind.StringLiteralExpression &&
						_.Kind() != SyntaxKind.NumericLiteralExpression &&
						_.Kind() != SyntaxKind.ObjectCreationExpression))
				{
					var variableDeclaration = node.ChildNodes()
						.Single(_ => _.Kind() == SyntaxKind.VariableDeclaration);
					var identifierName = variableDeclaration.ChildNodes()
						.SingleOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName);

					return identifierName != null ?
						new CSharpStyleVarElsewhereStyle(this.Data.Update((identifierName as IdentifierNameSyntax).IsVar), this.Severity) :
						new CSharpStyleVarElsewhereStyle(this.Data.Update(false), this.Severity);
				}
				else
				{
					return new CSharpStyleVarElsewhereStyle(this.Data, this.Severity);
				}
			}
			else
			{
				return new CSharpStyleVarElsewhereStyle(this.Data, this.Severity);
			}
		}
	}
}