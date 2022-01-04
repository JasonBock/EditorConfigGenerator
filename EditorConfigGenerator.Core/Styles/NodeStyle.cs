using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public abstract class NodeStyle<TData, TNode, TNodeInfo, TStyle>
	 where TData : StatisticalData<TData>
	 where TNode : SyntaxNode
	 where TNodeInfo : NodeInformation<TNode>
	 where TStyle : NodeStyle<TData, TNode, TNodeInfo, TStyle>
{
	protected NodeStyle(TData data) =>
		this.Data = data ?? throw new ArgumentNullException(nameof(data));

	public abstract TStyle Add(TStyle style);

	public abstract string GetSetting();

	public abstract TStyle Update(TNodeInfo information);

	public TData Data { get; }
}