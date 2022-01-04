﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpStyleVarForBuiltInTypesStyle
	: SeverityNodeStyle<BooleanData, LocalDeclarationStatementSyntax, NodeInformation<LocalDeclarationStatementSyntax>, CSharpStyleVarForBuiltInTypesStyle>
{
	public const string Setting = "csharp_style_var_for_built_in_types";

	public CSharpStyleVarForBuiltInTypesStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpStyleVarForBuiltInTypesStyle Add(CSharpStyleVarForBuiltInTypesStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpStyleVarForBuiltInTypesStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{CSharpStyleVarForBuiltInTypesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override CSharpStyleVarForBuiltInTypesStyle Update(NodeInformation<LocalDeclarationStatementSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			if (node.DescendantNodes()
				.Any(_ => _.Kind() == SyntaxKind.StringLiteralExpression ||
					_.Kind() == SyntaxKind.NumericLiteralExpression))
			{
				var variableDeclaration = node.ChildNodes()
					.Single(_ => _.Kind() == SyntaxKind.VariableDeclaration);
				var identifierName = variableDeclaration.ChildNodes()
					.SingleOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName);

				return identifierName is not null ?
					new CSharpStyleVarForBuiltInTypesStyle(this.Data.Update(((IdentifierNameSyntax)identifierName).IsVar), this.Severity) :
					new CSharpStyleVarForBuiltInTypesStyle(this.Data.Update(false), this.Severity);
			}
		}

		return new CSharpStyleVarForBuiltInTypesStyle(this.Data, this.Severity);
	}
}