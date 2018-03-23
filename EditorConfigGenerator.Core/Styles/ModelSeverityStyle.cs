using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class ModelSeverityStyle<TData, TNode, TStyle>
		: ModelStyle<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : ModelSeverityStyle<TData, TNode, TStyle>
	{
		protected ModelSeverityStyle(TData data, SemanticModel model, Severity severity = Severity.Error)
			: base(data, model) => this.Severity = severity;

		public Severity Severity { get; }
	}
}