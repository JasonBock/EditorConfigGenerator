using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class Style<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : Style<TData, TNode, TStyle>
	{
		protected Style(TData data, Severity severity = Severity.Error)
		{
			this.Data = data;
			this.Severity = severity;
		}

		public abstract TStyle Add(TStyle style);

		public abstract string GetSetting();

		public abstract TStyle Update(TNode node);

		public TData Data { get; }
		public Severity Severity { get; }
	}
}
