using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceBetweenParenthesesStyle
		: Style<ParenthesesSpaceData, SyntaxNode, NodeInformation<SyntaxNode>, CSharpSpaceBetweenParenthesesStyle>
	{
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
					items.Add("control_flow_statements");
				}

				if (this.Data.ExpressionsSpaceOccurences > this.Data.ExpressionsNoSpaceOccurences)
				{
					items.Add("expressions");
				}

				if (this.Data.TypeCastsSpaceOccurences > this.Data.TypeCastsNoSpaceOccurences)
				{
					items.Add("type_casts");
				}

				return items.Count > 0 ?
					$"csharp_space_between_parentheses = {string.Join(", ", items)}" : string.Empty;
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
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateControlFlow(
						CSharpSpaceBetweenParenthesesStyle.HasParenthesisSpacing(node)));
				}
				else if(node is ParenthesizedExpressionSyntax)
				{
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateExpression(
						CSharpSpaceBetweenParenthesesStyle.HasParenthesisSpacing(node)));
				}
				else if (node is CastExpressionSyntax)
				{
					return new CSharpSpaceBetweenParenthesesStyle(this.Data.UpdateTypeCast(
						CSharpSpaceBetweenParenthesesStyle.HasParenthesisSpacing(node)));
				}
			}

			return new CSharpSpaceBetweenParenthesesStyle(this.Data);
		}

		private static bool HasParenthesisSpacing(SyntaxNode node)
		{
			var children = node.ChildNodesAndTokens().ToArray();
			var openParen = (SyntaxToken?)children.FirstOrDefault(_ => _.IsKind(SyntaxKind.OpenParenToken));
			var closeParen = (SyntaxToken?)children.LastOrDefault(_ => _.IsKind(SyntaxKind.CloseParenToken));

			var hasSpaceAfterOpenParen = openParen != null && openParen.Value.HasTrailingTrivia &&
				openParen.Value.TrailingTrivia.Any(_ => _.IsKind(SyntaxKind.WhitespaceTrivia));

			if(hasSpaceAfterOpenParen && closeParen != null)
			{
				var previousNodeIndex = Array.IndexOf(children, closeParen.Value) - 1;
				var lastNodeOrToken = CSharpSpaceBetweenParenthesesStyle.GetLastNodeOrToken(children[previousNodeIndex]);

				var hasSpaceBeforeCloseParen = lastNodeOrToken.HasTrailingTrivia &&
					lastNodeOrToken.GetTrailingTrivia().Any(_ => _.IsKind(SyntaxKind.WhitespaceTrivia));

				return hasSpaceBeforeCloseParen;
			}

			return false;
		}

		private static SyntaxNodeOrToken GetLastNodeOrToken(SyntaxNodeOrToken nodeOrToken)
		{
			SyntaxNodeOrToken last = default;
			var children = nodeOrToken.ChildNodesAndTokens().ToArray();

			while (children.Length > 0)
			{
				last = children[children.Length - 1];
				children = last.ChildNodesAndTokens().ToArray();
			}

			return last;
		}
	}
}
