using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpNewLineBetweenQueryExpressionClausesStyle
		: SeverityNodeStyle<BooleanData, QueryExpressionSyntax, NodeInformation<QueryExpressionSyntax>, CSharpNewLineBetweenQueryExpressionClausesStyle>
	{
		public const string Setting = "csharp_new_line_between_query_expression_clauses";

		public CSharpNewLineBetweenQueryExpressionClausesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpNewLineBetweenQueryExpressionClausesStyle Add(CSharpNewLineBetweenQueryExpressionClausesStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpNewLineBetweenQueryExpressionClausesStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpNewLineBetweenQueryExpressionClausesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpNewLineBetweenQueryExpressionClausesStyle Update(NodeInformation<QueryExpressionSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var clauseCount = node.DescendantNodes().Count(_ => _ is QueryClauseSyntax || _ is SelectOrGroupClauseSyntax);
				var eolCount = node.DescendantTrivia().Count(_ => _.IsKind(SyntaxKind.EndOfLineTrivia));
				return new CSharpNewLineBetweenQueryExpressionClausesStyle(this.Data.Update(eolCount >= clauseCount - 1), this.Severity);
			}

			return new CSharpNewLineBetweenQueryExpressionClausesStyle(this.Data, this.Severity);
		}
	}
}
