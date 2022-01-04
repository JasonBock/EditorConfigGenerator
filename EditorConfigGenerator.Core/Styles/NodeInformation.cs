using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public class NodeInformation<TNode>
	where TNode : SyntaxNode
{
	public NodeInformation(TNode node) =>
		this.Node = node ?? throw new ArgumentNullException(nameof(node));

	public static implicit operator NodeInformation<TNode>(TNode node) => new NodeInformation<TNode>(node);

	public static explicit operator TNode(NodeInformation<TNode> information)
	{
		if (information is null)
		{
			throw new ArgumentNullException(nameof(information));
		}

		return information.Node;
	}

	public TNode Node { get; }
}