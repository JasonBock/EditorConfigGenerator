using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SpecialNameExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class DotnetStylePredefinedTypeForMemberAccessStyle
	: ModelSeverityNodeStyle<BooleanData, MemberAccessExpressionSyntax, ModelNodeInformation<MemberAccessExpressionSyntax>, DotnetStylePredefinedTypeForMemberAccessStyle>
{
	public const string Setting = "dotnet_style_predefined_type_for_member_access";

	public DotnetStylePredefinedTypeForMemberAccessStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override DotnetStylePredefinedTypeForMemberAccessStyle Add(DotnetStylePredefinedTypeForMemberAccessStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new DotnetStylePredefinedTypeForMemberAccessStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{DotnetStylePredefinedTypeForMemberAccessStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override DotnetStylePredefinedTypeForMemberAccessStyle Update(ModelNodeInformation<MemberAccessExpressionSyntax> information)
	{
		var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

		if (!node.ContainsDiagnostics)
		{
			if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.PredefinedType))
			{
				return new DotnetStylePredefinedTypeForMemberAccessStyle(
					this.Data.Update(true), this.Severity);
			}
			else
			{
				if (node.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName) is IdentifierNameSyntax identifierNode)
				{
					if (model.GetSymbolInfo(identifierNode).Symbol is ITypeSymbol identifierType &&
						identifierType.SpecialType.IsPredefinedType())
					{
						return new DotnetStylePredefinedTypeForMemberAccessStyle(
							this.Data.Update(false), this.Severity);
					}
				}
			}
		}

		return new DotnetStylePredefinedTypeForMemberAccessStyle(this.Data, this.Severity);
	}
}