using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceBetweenParenthesesStyle
		: NodeStyle<ParenthesesSpaceData, SyntaxNode, NodeInformation<SyntaxNode>, CSharpSpaceBetweenParenthesesStyle>
	{
		public const string Setting = "csharp_space_between_parentheses";
		public const string ControlFlow = "control_flow_statements";
		public const string Expressions = "expressions";
		public const string TypeCasts = "type_casts";

		public CSharpSpaceBetweenParenthesesStyle(ParenthesesSpaceData data)
			: base(data) { }

		public override CSharpSpaceBetweenParenthesesStyle Add(CSharpSpaceBetweenParenthesesStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpSpaceBetweenParenthesesStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var items = new List<string>();

				if (this.Data.ControlFlowSpaceOccurences > this.Data.ControlFlowNoSpaceOccurences)
				{
					items.Add(CSharpSpaceBetweenParenthesesStyle.ControlFlow);
				}

				if (this.Data.ExpressionsSpaceOccurences > this.Data.ExpressionsNoSpaceOccurences)
				{
					items.Add(CSharpSpaceBetweenParenthesesStyle.Expressions);
				}

				if (this.Data.TypeCastsSpaceOccurences > this.Data.TypeCastsNoSpaceOccurences)
				{
					items.Add(CSharpSpaceBetweenParenthesesStyle.TypeCasts);
				}

				return items.Count > 0 ?
					$"{CSharpSpaceBetweenParenthesesStyle.Setting} = {string.Join(", ", items)}" : string.Empty;
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpSpaceBetweenParenthesesStyle Update(NodeInformation<SyntaxNode> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				if(node is ForStatementSyntax || node is ForEachStatementSyntax || node is IfStatementSyntax || 
					node is SwitchStatementSyntax || node is WhileStatementSyntax)
				{
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateControlFlow(node.HasParenthesisSpacing()));
				}
				else if(node is ParenthesizedExpressionSyntax)
				{
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateExpression(node.HasParenthesisSpacing()));
				}
				else if (node is CastExpressionSyntax)
				{
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateTypeCast(node.HasParenthesisSpacing()));
				}
			}

			return new CSharpSpaceBetweenParenthesesStyle(this.Data);
		}
	}
}