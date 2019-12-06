using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle
		: SeverityNodeStyle<BooleanData, ParameterListSyntax, NodeInformation<ParameterListSyntax>, CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle>
	{
		public const string Setting = "csharp_space_between_method_declaration_parameter_list_parentheses";

		public CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle Add(CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle Update(NodeInformation<ParameterListSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				return new CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle(
					this.Data.Update(node.HasParenthesisSpacing()));
			}

			return new CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle(this.Data);
		}
	}
}