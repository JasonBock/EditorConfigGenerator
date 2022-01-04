using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class DotnetStyleExplicitTupleNamesStyle
	: ModelSeverityNodeStyle<BooleanData, MemberAccessExpressionSyntax, ModelNodeInformation<MemberAccessExpressionSyntax>, DotnetStyleExplicitTupleNamesStyle>
{
	public const string Setting = "dotnet_style_explicit_tuple_names";

	public DotnetStyleExplicitTupleNamesStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override DotnetStyleExplicitTupleNamesStyle Add(DotnetStyleExplicitTupleNamesStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new DotnetStyleExplicitTupleNamesStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{DotnetStyleExplicitTupleNamesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override DotnetStyleExplicitTupleNamesStyle Update(ModelNodeInformation<MemberAccessExpressionSyntax> information)
	{
		var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

		if (!node.ContainsDiagnostics)
		{
			if (model.GetSymbolInfo(node).Symbol is IFieldSymbol field)
			{
				var correspondingTupleField = field.CorrespondingTupleField;

				if (correspondingTupleField is not null)
				{
					return new DotnetStyleExplicitTupleNamesStyle(this.Data.Update(
						correspondingTupleField.Name != field.Name), this.Severity);
				}
			}
		}

		return new DotnetStyleExplicitTupleNamesStyle(this.Data, this.Severity);
	}
}