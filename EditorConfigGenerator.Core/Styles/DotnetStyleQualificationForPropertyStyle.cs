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
	public sealed class DotnetStyleQualificationForPropertyStyle
		: ModelSeverityNodeStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, DotnetStyleQualificationForPropertyStyle>
	{
		public const string Setting = "dotnet_style_qualification_for_property";

		public DotnetStyleQualificationForPropertyStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleQualificationForPropertyStyle Add(DotnetStyleQualificationForPropertyStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleQualificationForPropertyStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{DotnetStyleQualificationForPropertyStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleQualificationForPropertyStyle Update(ModelNodeInformation<SyntaxNode> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if(node is MemberAccessExpressionSyntax || node is IdentifierNameSyntax)
				{
					if (model.GetSymbolInfo(node).Symbol is IPropertySymbol propertySymbol)
					{
						if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression))
						{
							return new DotnetStyleQualificationForPropertyStyle(this.Data.Update(true), this.Severity);
						}
						else
						{
							var classNode = node.FindParent<ClassDeclarationSyntax>();

							if (!propertySymbol.IsStatic && classNode != null &&
								model.GetDeclaredSymbol(classNode) == propertySymbol.ContainingType)
							{
								return new DotnetStyleQualificationForPropertyStyle(this.Data.Update(false), this.Severity);
							}

							return new DotnetStyleQualificationForPropertyStyle(this.Data, this.Severity);
						}
					}
				}
			}

			return new DotnetStyleQualificationForPropertyStyle(this.Data, this.Severity);
		}
	}
}