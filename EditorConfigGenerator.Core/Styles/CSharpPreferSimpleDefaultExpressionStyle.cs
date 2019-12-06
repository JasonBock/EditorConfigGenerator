using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpPreferSimpleDefaultExpressionStyle
		: SeverityNodeStyle<BooleanData, ExpressionSyntax, NodeInformation<ExpressionSyntax>, CSharpPreferSimpleDefaultExpressionStyle>
	{
		public const string Setting = "csharp_prefer_simple_default_expression";

		public CSharpPreferSimpleDefaultExpressionStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpPreferSimpleDefaultExpressionStyle Add(CSharpPreferSimpleDefaultExpressionStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpPreferSimpleDefaultExpressionStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpPreferSimpleDefaultExpressionStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpPreferSimpleDefaultExpressionStyle Update(NodeInformation<ExpressionSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				if (node is LiteralExpressionSyntax)
				{
					return new CSharpPreferSimpleDefaultExpressionStyle(this.Data.Update(true), this.Severity);
				}
				else if (node is DefaultExpressionSyntax)
				{
					return new CSharpPreferSimpleDefaultExpressionStyle(this.Data.Update(false), this.Severity);
				}
			}

			return new CSharpPreferSimpleDefaultExpressionStyle(this.Data, this.Severity);
		}
	}
}