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
	public sealed class DotnetStyleQualificationForFieldStyle
		: ModelSeverityNodeStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, DotnetStyleQualificationForFieldStyle>
	{
		public const string Setting = "dotnet_style_qualification_for_field";

		public DotnetStyleQualificationForFieldStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleQualificationForFieldStyle Add(DotnetStyleQualificationForFieldStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleQualificationForFieldStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{DotnetStyleQualificationForFieldStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleQualificationForFieldStyle Update(ModelNodeInformation<SyntaxNode> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if(node is MemberAccessExpressionSyntax || node is IdentifierNameSyntax)
				{
					if (model.GetSymbolInfo(node).Symbol is IFieldSymbol fieldSymbol)
					{
						if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression))
						{
							return new DotnetStyleQualificationForFieldStyle(this.Data.Update(true), this.Severity);
						}
						else
						{
							var classNode = node.FindParent<ClassDeclarationSyntax>();

							if (!fieldSymbol.IsStatic && classNode is { } &&
								model.GetDeclaredSymbol(classNode).Equals(fieldSymbol.ContainingType))
							{
								return new DotnetStyleQualificationForFieldStyle(this.Data.Update(false), this.Severity);
							}

							return new DotnetStyleQualificationForFieldStyle(this.Data, this.Severity);
						}
					}
				}
			}

			return new DotnetStyleQualificationForFieldStyle(this.Data, this.Severity);
		}
	}
}