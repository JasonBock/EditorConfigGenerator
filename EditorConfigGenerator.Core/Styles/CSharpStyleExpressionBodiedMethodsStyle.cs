using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedMethodsStyle
		: SeverityStyle<ExpressionBodiedData, MethodDeclarationSyntax, NodeInformation<MethodDeclarationSyntax>, CSharpStyleExpressionBodiedMethodsStyle>
	{
		public CSharpStyleExpressionBodiedMethodsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedMethodsStyle Add(CSharpStyleExpressionBodiedMethodsStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedMethodsStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting("csharp_style_expression_bodied_methods", this.Severity);

		public override CSharpStyleExpressionBodiedMethodsStyle Update(NodeInformation<MethodDeclarationSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }
			return new CSharpStyleExpressionBodiedMethodsStyle(information.Node.Examine(this.Data), this.Severity);
		}
	}
}