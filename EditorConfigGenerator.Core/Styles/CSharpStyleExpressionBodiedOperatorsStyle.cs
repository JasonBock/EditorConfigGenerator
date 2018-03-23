﻿using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedOperatorsStyle
		: SeverityStyle<ExpressionBodiedData, OperatorDeclarationSyntax, CSharpStyleExpressionBodiedOperatorsStyle>
	{
		public CSharpStyleExpressionBodiedOperatorsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedOperatorsStyle Add(CSharpStyleExpressionBodiedOperatorsStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedOperatorsStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() => 
			this.Data.GetSetting("csharp_style_expression_bodied_operators", this.Severity);

		public override CSharpStyleExpressionBodiedOperatorsStyle Update(OperatorDeclarationSyntax node) => 
			new CSharpStyleExpressionBodiedOperatorsStyle(node.Examine(this.Data), this.Severity);
	}
}