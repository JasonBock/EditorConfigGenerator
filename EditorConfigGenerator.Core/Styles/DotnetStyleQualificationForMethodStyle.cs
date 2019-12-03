using EditorConfigGenerator.Core.Extensions;
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
		: ModelSeverityNodeStyle<BooleanData, InvocationExpressionSyntax, ModelNodeInformation<InvocationExpressionSyntax>, DotnetStyleQualificationForMethodStyle>
	{
		public const string Setting = "dotnet_style_qualification_for_method";

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
				return $"{DotnetStyleQualificationForMethodStyle.Setting} = {value}:{this.Severity.GetDescription()}";
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
				if (model.GetSymbolInfo(node).Symbol is IMethodSymbol methodSymbol)
				{
					if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression))
					{
						return new DotnetStyleQualificationForMethodStyle(this.Data.Update(true), this.Severity);
					}
					else
					{
						var classNode = node.FindParent<ClassDeclarationSyntax>();

						if (!methodSymbol.IsStatic && classNode is { } &&
							object.ReferenceEquals(model.GetDeclaredSymbol(classNode), methodSymbol.ContainingType) &&
							!methodSymbol.IsExtensionMethod)
						{
							return new DotnetStyleQualificationForMethodStyle(this.Data.Update(false), this.Severity);
						}

						return new DotnetStyleQualificationForMethodStyle(this.Data, this.Severity);
					}
				}
			}

			return new DotnetStyleQualificationForMethodStyle(this.Data, this.Severity);
		}
	}
}