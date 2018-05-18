using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStylePatternMatchingOverAsWithNullCheckStyle
		: SeverityNodeStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, CSharpStylePatternMatchingOverAsWithNullCheckStyle>
	{
		public const string Setting = "csharp_style_pattern_matching_over_as_with_null_check";

		public CSharpStylePatternMatchingOverAsWithNullCheckStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStylePatternMatchingOverAsWithNullCheckStyle Add(CSharpStylePatternMatchingOverAsWithNullCheckStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpStylePatternMatchingOverAsWithNullCheckStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStylePatternMatchingOverAsWithNullCheckStyle Update(ModelNodeInformation<SyntaxNode> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if (node is IsPatternExpressionSyntax && node.FindParent<IfStatementSyntax>() != null)
				{
					return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data.Update(true), this.Severity);
				}
				else if (node is IfStatementSyntax)
				{
					foreach (var binary in node.ChildNodes().Where(_ => _ is BinaryExpressionSyntax))
					{
						if (binary.ChildNodes().Any(_ => _.IsKind(SyntaxKind.NullLiteralExpression)))
						{
							// harder than this thought. The "thing" being compared to null 
							// has to have an IdentifierName that resolves to a symbol, and
							// it had to have some kind of assignment via an AsExpression before the IfStatement.
							if (binary.DescendantNodes().FirstOrDefault(_ => _.IsKind(SyntaxKind.IdentifierName)) is IdentifierNameSyntax identifier)
							{
								if (CSharpStylePatternMatchingOverAsWithNullCheckStyle.IsIdentifierUsedWithAsExpression(
									identifier, node, model))
								{
									return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data.Update(false), this.Severity);
								}
							}
						}
					}
				}
			}

			return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data, this.Severity);
		}

		private static bool IsIdentifierUsedWithAsExpression(IdentifierNameSyntax identifier, SyntaxNode node, SemanticModel model)
		{
			var identifierSymbol = model.GetSymbolInfo(identifier).Symbol;

			if (identifierSymbol != null)
			{
				var parent = node.Parent;
				var children = parent.ChildNodes().ToImmutableArray();
				var nodeIndex = children.IndexOf(node);

				// So, go to the parent of node, get its child nodes, find the index of node,
				// and then go through those children in reverse from index - 1. Find the "last"
				// identifier whose symbol is the same as 
				for (var i = nodeIndex - 1; i >= 0; i--)
				{
					var childNode = children[i];

					if(childNode.DescendantNodes().Any(_ => _.IsKind(SyntaxKind.AsExpression)))
					{
						var childIdentifier = childNode.DescendantNodes().FirstOrDefault(
							_ => _ is IdentifierNameSyntax && model.GetSymbolInfo(_).Symbol == identifierSymbol);

						if (childIdentifier != null)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		// Look for IsPatternExpression within an IfStatement, if it is, true
		// The second....just do a IfStatement with a child
		// EqualsExpression or NotEqualsExpression, and a child 
		// NullLiteralExpression, then it's false
		// Note, a WhileStatement does this too, but I don't think
		// that's what this style is trying to represent.
		public void Foo(string o)
		{
			// csharp_style_pattern_matching_over_as_with_null_check = true
			if (o is string s) { }

			// csharp_style_pattern_matching_over_as_with_null_check = false
			var r = o as string;
			int x = 22;
			int y = x++;
			if (r != null) { }

			this.arr = o as string;
			if (this.arr != null) { }
		}

		private string arr;
	}
}
