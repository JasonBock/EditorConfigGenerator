using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpStyleExpressionBodiedOperatorsStyle
	: SeverityNodeStyle<ExpressionBodiedData, OperatorDeclarationSyntax, NodeInformation<OperatorDeclarationSyntax>, CSharpStyleExpressionBodiedOperatorsStyle>
{
	public const string Setting = "csharp_style_expression_bodied_operators";

	public CSharpStyleExpressionBodiedOperatorsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpStyleExpressionBodiedOperatorsStyle Add(CSharpStyleExpressionBodiedOperatorsStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpStyleExpressionBodiedOperatorsStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting() =>
		this.Data.GetSetting(CSharpStyleExpressionBodiedOperatorsStyle.Setting, this.Severity);

	public override CSharpStyleExpressionBodiedOperatorsStyle Update(NodeInformation<OperatorDeclarationSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }
		return new CSharpStyleExpressionBodiedOperatorsStyle(information.Node.Examine(this.Data), this.Severity);
	}
}