using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class SeverityStyle<TData, TNode, TStyle>
		: Style<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : SeverityStyle<TData, TNode, TStyle>
	{
		protected SeverityStyle(TData data, Severity severity = Severity.Error)
			: base(data) => this.Severity = severity;

		public Severity Severity { get; }
	}
}