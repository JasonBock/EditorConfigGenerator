using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedMethodsStyle
		: SeverityStyle<ExpressionBodiedData, MethodDeclarationSyntax, CSharpStyleExpressionBodiedMethodsStyle>
	{
		public CSharpStyleExpressionBodiedMethodsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedMethodsStyle Add(CSharpStyleExpressionBodiedMethodsStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedMethodsStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting() => 
			this.Data.GetSetting("csharp_style_expression_bodied_methods", this.Severity);

		public override CSharpStyleExpressionBodiedMethodsStyle Update(MethodDeclarationSyntax node) => 
			new CSharpStyleExpressionBodiedMethodsStyle(node.Examine(this.Data));
	}
}