using Microsoft.CodeAnalysis;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public class ModelNodeInformation<TNode>
		: NodeInformation<TNode>
		where TNode : SyntaxNode
	{
		public ModelNodeInformation(TNode node, SemanticModel model) 
			: base(node) =>
			this.Model = model ?? throw new ArgumentNullException(nameof(model));

		public void Deconstruct(out TNode node, out SemanticModel model)
		{
			node = this.Node;
			model = this.Model;
		}

		public SemanticModel Model { get; }
	}
}
