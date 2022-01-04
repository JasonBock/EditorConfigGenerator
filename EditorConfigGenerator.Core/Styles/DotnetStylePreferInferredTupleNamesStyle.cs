﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class DotnetStylePreferInferredTupleNamesStyle
	: SeverityNodeStyle<BooleanData, TupleExpressionSyntax, NodeInformation<TupleExpressionSyntax>, DotnetStylePreferInferredTupleNamesStyle>
{
	public const string Setting = "dotnet_style_prefer_inferred_tuple_names";

	public DotnetStylePreferInferredTupleNamesStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override DotnetStylePreferInferredTupleNamesStyle Add(DotnetStylePreferInferredTupleNamesStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new DotnetStylePreferInferredTupleNamesStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{DotnetStylePreferInferredTupleNamesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override DotnetStylePreferInferredTupleNamesStyle Update(NodeInformation<TupleExpressionSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			var data = this.Data;

			foreach (var argument in node.ChildNodes().OfType<ArgumentSyntax>())
			{
				data = data.Update(!argument.ChildNodes().Any(_ => _.Kind() == SyntaxKind.NameColon));
			}

			return new DotnetStylePreferInferredTupleNamesStyle(data, this.Severity);
		}

		return new DotnetStylePreferInferredTupleNamesStyle(this.Data, this.Severity);
	}
}