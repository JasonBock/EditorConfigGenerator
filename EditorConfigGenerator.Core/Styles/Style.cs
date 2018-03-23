using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class Style<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : Style<TData, TNode, TStyle>
	{
		protected Style(TData data) => 
			this.Data = data ?? throw new ArgumentNullException(nameof(data));

		public abstract TStyle Add(TStyle style);

		public abstract string GetSetting();

		public abstract TStyle Update(TNode node);

		public TData Data { get; }
	}
}
