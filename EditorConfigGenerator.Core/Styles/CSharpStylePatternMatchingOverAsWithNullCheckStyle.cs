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
				if (node is IsPatternExpressionSyntax && node.FindParent<IfStatementSyntax>() is { })
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

			if (identifierSymbol is { })
			{
				var parent = node.Parent;
				var children = parent.ChildNodes().ToImmutableArray();
				var nodeIndex = children.IndexOf(node);

				for (var i = nodeIndex - 1; i >= 0; i--)
				{
					var childNode = children[i];
					var childDescendants = childNode.DescendantNodes().ToImmutableArray();

					if(childDescendants.Any(_ => _.IsKind(SyntaxKind.AsExpression)) &&
						childDescendants.FirstOrDefault(
							c => model.GetDeclaredSymbol(c)?.Equals(identifierSymbol) ?? false) is { })
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
