using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedMethodsStyle
		: SeverityStyle<BooleanData, MethodDeclarationSyntax, CSharpStyleExpressionBodiedMethodsStyle>
	{
		public CSharpStyleExpressionBodiedMethodsStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedMethodsStyle Add(CSharpStyleExpressionBodiedMethodsStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedMethodsStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_style_expression_bodied_methods = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleExpressionBodiedMethodsStyle Update(MethodDeclarationSyntax node) => 
			new CSharpStyleExpressionBodiedMethodsStyle(node.Examine(this.Data));
	}
}