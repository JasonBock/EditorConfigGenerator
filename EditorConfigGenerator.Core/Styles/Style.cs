using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class Style<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : Style<TData, TNode, TStyle>
	{
		protected Style(TData data) => this.Data = data;

		public abstract TStyle Add(TStyle style);

		public abstract TStyle Update(TNode node);

		public abstract string Setting { get; }
		public TData Data { get; }
	}
}
