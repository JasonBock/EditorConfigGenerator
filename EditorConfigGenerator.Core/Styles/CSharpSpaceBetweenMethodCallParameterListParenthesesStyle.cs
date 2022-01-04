using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles;

public class CSharpSpaceBetweenMethodCallParameterListParenthesesStyle
	: SeverityNodeStyle<BooleanData, ArgumentListSyntax, NodeInformation<ArgumentListSyntax>, CSharpSpaceBetweenMethodCallParameterListParenthesesStyle>
{
	public const string Setting = "csharp_space_between_method_call_parameter_list_parentheses";

	public CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpSpaceBetweenMethodCallParameterListParenthesesStyle Add(CSharpSpaceBetweenMethodCallParameterListParenthesesStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(this.Data.Add(style.Data));
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override CSharpSpaceBetweenMethodCallParameterListParenthesesStyle Update(NodeInformation<ArgumentListSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			return new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(
				this.Data.Update(node.HasParenthesisSpacing()));
		}

		return new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(this.Data);
	}
}