using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public abstract class SeverityNodeStyle<TData, TNode, TNodeInfo, TStyle>
	: NodeStyle<TData, TNode, TNodeInfo, TStyle>
	where TData : StatisticalData<TData>
	where TNode : SyntaxNode
	where TNodeInfo : NodeInformation<TNode>
	where TStyle : SeverityNodeStyle<TData, TNode, TNodeInfo, TStyle>
{
	protected SeverityNodeStyle(TData data, Severity severity = Severity.Error)
		: base(data) => this.Severity = severity;

	public Severity Severity { get; }
}