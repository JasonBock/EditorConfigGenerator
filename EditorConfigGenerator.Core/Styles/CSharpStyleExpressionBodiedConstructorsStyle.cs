using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
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

		public override CSharpStyleExpressionBodiedConstructorsStyle Update(
			ConstructorDeclarationSyntax node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				var arrowExpressionExists = node.DescendantNodes()
					.Any(_ => _.Kind() == SyntaxKind.ArrowExpressionClause);

				if(arrowExpressionExists)
				{
					return new CSharpStyleExpressionBodiedConstructorsStyle(this.Data.Update(true));
				}
				else
				{
					var statementSyntaxCount = node.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.Block)
						?.DescendantNodes().Count(_ => typeof(StatementSyntax).IsAssignableFrom(_.GetType()));

					return statementSyntaxCount == 1 ?
						new CSharpStyleExpressionBodiedConstructorsStyle(this.Data.Update(false)) :
						this;
				}
			}
			else
			{
				return this;
			}
		}
	}
}