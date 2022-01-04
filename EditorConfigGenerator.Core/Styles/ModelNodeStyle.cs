using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public abstract class ModelNodeStyle<TData, TNode, TNodeInfo, TStyle>
	: NodeStyle<TData, TNode, TNodeInfo, TStyle>
	where TData : StatisticalData<TData>
	where TNode : SyntaxNode
	where TNodeInfo : ModelNodeInformation<TNode>
	where TStyle : NodeStyle<TData, TNode, TNodeInfo, TStyle>
{
	protected ModelNodeStyle(TData data)
		: base(data) { }
}