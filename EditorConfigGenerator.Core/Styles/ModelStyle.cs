using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class ModelStyle<TData, TNode, TNodeInfo, TStyle>
		: Style<TData, TNode, TNodeInfo, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TNodeInfo : ModelNodeInformation<TNode>
		where TStyle : Style<TData, TNode, TNodeInfo, TStyle>
	{
		protected ModelStyle(TData data)
			: base(data) { }
	}
}
