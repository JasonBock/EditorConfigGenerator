using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStyleQualificationForMethodStyle
		: ModelSeverityStyle<BooleanData, InvocationExpressionSyntax, ModelNodeInformation<InvocationExpressionSyntax>, DotnetStyleQualificationForMethodStyle>
	{
		public DotnetStyleQualificationForMethodStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleQualificationForMethodStyle Add(DotnetStyleQualificationForMethodStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleQualificationForMethodStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_style_qualification_for_method = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleQualificationForMethodStyle Update(ModelNodeInformation<InvocationExpressionSyntax> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				var method = model.GetSymbolInfo(node).Symbol as IMethodSymbol;
				var simpleMember = node.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.SimpleMemberAccessExpression);

				if (simpleMember != null)
				{
					if (simpleMember.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression) && 
						(!method?.IsExtensionMethod ?? false))
					{
						return new DotnetStyleQualificationForMethodStyle(this.Data.Update(true), this.Severity);
					}
				}
				else if (!method?.IsStatic ?? false)
				{
					return new DotnetStyleQualificationForMethodStyle(this.Data.Update(false), this.Severity);
				}
			}

			return new DotnetStyleQualificationForMethodStyle(this.Data, this.Severity);
		}
	}
}