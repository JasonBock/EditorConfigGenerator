﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class DotnetStyleQualificationForEventStyle
	: ModelSeverityNodeStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, DotnetStyleQualificationForEventStyle>
{
	public const string Setting = "dotnet_style_qualification_for_event";

	public DotnetStyleQualificationForEventStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override DotnetStyleQualificationForEventStyle Add(DotnetStyleQualificationForEventStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new DotnetStyleQualificationForEventStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{DotnetStyleQualificationForEventStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override DotnetStyleQualificationForEventStyle Update(ModelNodeInformation<SyntaxNode> information)
	{
		var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

		if (!node.ContainsDiagnostics)
		{
			if (node is MemberAccessExpressionSyntax || node is IdentifierNameSyntax)
			{
				if (model.GetSymbolInfo(node).Symbol is IEventSymbol eventSymbol)
				{
					if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression))
					{
						return new DotnetStyleQualificationForEventStyle(this.Data.Update(true), this.Severity);
					}
					else
					{
						var classNode = node.FindParent<ClassDeclarationSyntax>();

						if (!eventSymbol.IsStatic && classNode is not null &&
							object.ReferenceEquals(model.GetDeclaredSymbol(classNode), eventSymbol.ContainingType))
						{
							return new DotnetStyleQualificationForEventStyle(this.Data.Update(false), this.Severity);
						}

						return new DotnetStyleQualificationForEventStyle(this.Data, this.Severity);
					}
				}
			}
		}

		return new DotnetStyleQualificationForEventStyle(this.Data, this.Severity);
	}
}