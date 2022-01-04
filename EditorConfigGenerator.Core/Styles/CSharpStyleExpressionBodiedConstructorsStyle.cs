using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpStyleExpressionBodiedConstructorsStyle
	: SeverityNodeStyle<ExpressionBodiedData, ConstructorDeclarationSyntax, NodeInformation<ConstructorDeclarationSyntax>, CSharpStyleExpressionBodiedConstructorsStyle>
{
	public const string Setting = "csharp_style_expression_bodied_constructors";

	public CSharpStyleExpressionBodiedConstructorsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpStyleExpressionBodiedConstructorsStyle Add(CSharpStyleExpressionBodiedConstructorsStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpStyleExpressionBodiedConstructorsStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting() =>
		this.Data.GetSetting(CSharpStyleExpressionBodiedConstructorsStyle.Setting, this.Severity);

	public override CSharpStyleExpressionBodiedConstructorsStyle Update(NodeInformation<ConstructorDeclarationSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }
		return new CSharpStyleExpressionBodiedConstructorsStyle(information.Node.Examine(this.Data), this.Severity);
	}
}