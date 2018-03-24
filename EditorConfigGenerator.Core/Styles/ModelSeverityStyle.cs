using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class ModelSeverityStyle<TData, TNode, TNodeInfo, TStyle>
		: ModelStyle<TData, TNode, TNodeInfo, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TNodeInfo : ModelNodeInformation<TNode>
		where TStyle : ModelSeverityStyle<TData, TNode, TNodeInfo, TStyle>
	{
		protected ModelSeverityStyle(TData data, Severity severity = Severity.Error)
			: base(data) => this.Severity = severity;

		public Severity Severity { get; }
	}
}