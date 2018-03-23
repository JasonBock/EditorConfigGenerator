using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedConstructorsStyle
		: SeverityStyle<ExpressionBodiedData, ConstructorDeclarationSyntax, CSharpStyleExpressionBodiedConstructorsStyle>
	{
		public CSharpStyleExpressionBodiedConstructorsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedConstructorsStyle Add(CSharpStyleExpressionBodiedConstructorsStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedConstructorsStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting("csharp_style_expression_bodied_constructors", this.Severity);

		public override CSharpStyleExpressionBodiedConstructorsStyle Update(ConstructorDeclarationSyntax node) => 
			new CSharpStyleExpressionBodiedConstructorsStyle(node.Examine(this.Data), this.Severity);
	}
}