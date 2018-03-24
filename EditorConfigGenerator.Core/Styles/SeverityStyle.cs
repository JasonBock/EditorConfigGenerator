using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class SeverityStyle<TData, TNode, TNodeInfo, TStyle>
		: Style<TData, TNode, TNodeInfo, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TNodeInfo : NodeInformation<TNode>
		where TStyle : SeverityStyle<TData, TNode, TNodeInfo, TStyle>
	{
		protected SeverityStyle(TData data, Severity severity = Severity.Error)
			: base(data) => this.Severity = severity;

		public Severity Severity { get; }
	}
}