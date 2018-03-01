using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core
{
	public abstract class Style<TStatistics, TNode, TStyle>
		where TStatistics : Statistics
		where TNode : SyntaxNode
		where TStyle : Style<TStatistics, TNode, TStyle>
	{
		protected Style(TStatistics statistics) => this.Statistics = statistics;

		public abstract TStyle Update(TNode node);

		public abstract string Setting { get; }
		public TStatistics Statistics { get; }
	}
}
