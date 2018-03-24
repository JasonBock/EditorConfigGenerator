using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class Style<TData, TNode, TNodeInfo, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TNodeInfo : NodeInformation<TNode>
		where TStyle : Style<TData, TNode, TNodeInfo, TStyle>
	{
		protected Style(TData data) => 
			this.Data = data ?? throw new ArgumentNullException(nameof(data));

		public abstract TStyle Add(TStyle style);

		public abstract string GetSetting();

		public abstract TStyle Update(TNodeInfo information);

		public TData Data { get; }
	}
}
