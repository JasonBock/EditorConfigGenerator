using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStylePatternMatchingOverAsWithNullCheckStyle
		: SeverityNodeStyle<BooleanData, SyntaxNode, NodeInformation<SyntaxNode>, CSharpStylePatternMatchingOverAsWithNullCheckStyle>
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

		public override CSharpStylePatternMatchingOverAsWithNullCheckStyle Update(NodeInformation<SyntaxNode> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				if(node is IsPatternExpressionSyntax && node.FindParent<IfStatementSyntax>() != null)
				{
					return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data.Update(true), this.Severity);
				}
				else if(node is IfStatementSyntax)
				{
					foreach (var binary in node.ChildNodes().Where(_ => _ is BinaryExpressionSyntax))
					{
						if (binary.ChildNodes().Any(_ => _.IsKind(SyntaxKind.NullLiteralExpression)))
						{
							// harder than thought. The "thing" being compared to null had
							// to have some kind of assignment via an AsExpression.
						}
					}
				}
			}

			return new CSharpStylePatternMatchingOverAsWithNullCheckStyle(this.Data, this.Severity);
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
			//var r = o;
			if (r != null) { }

			var qqq = new int[] { 1, 2, 3 };

			while(o is string ssss)
			{

			}
		}
	}
}
