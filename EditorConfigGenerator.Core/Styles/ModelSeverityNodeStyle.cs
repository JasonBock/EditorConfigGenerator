using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public abstract class ModelSeverityNodeStyle<TData, TNode, TNodeInfo, TStyle>
	: ModelNodeStyle<TData, TNode, TNodeInfo, TStyle>
	where TData : StatisticalData<TData>
	where TNode : SyntaxNode
	where TNodeInfo : ModelNodeInformation<TNode>
	where TStyle : ModelSeverityNodeStyle<TData, TNode, TNodeInfo, TStyle>
{
	protected ModelSeverityNodeStyle(TData data, Severity severity = Severity.Error)
		: base(data) => this.Severity = severity;

	public Severity Severity { get; }
}