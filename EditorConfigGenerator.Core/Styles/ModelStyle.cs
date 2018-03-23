using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class ModelStyle<TData, TNode, TStyle>
		: Style<TData, TNode, TStyle>
		where TData : Data<TData>
		where TNode : SyntaxNode
		where TStyle : Style<TData, TNode, TStyle>
	{
		protected ModelStyle(TData data, SemanticModel model)
			: base(data) => this.Model = model ?? throw new ArgumentNullException(nameof(model));

		public SemanticModel Model { get; }
	}
}
