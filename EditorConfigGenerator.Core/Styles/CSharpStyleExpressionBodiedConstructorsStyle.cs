using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedConstructorsStyle
		: SeverityStyle<BooleanData, ConstructorDeclarationSyntax, CSharpStyleExpressionBodiedConstructorsStyle>
	{
		public CSharpStyleExpressionBodiedConstructorsStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedConstructorsStyle Add(CSharpStyleExpressionBodiedConstructorsStyle style)
		{
			if(style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedConstructorsStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_style_expression_bodied_constructors = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleExpressionBodiedConstructorsStyle Update(ConstructorDeclarationSyntax node) => 
			new CSharpStyleExpressionBodiedConstructorsStyle(node.Examine(this.Data));
	}
}