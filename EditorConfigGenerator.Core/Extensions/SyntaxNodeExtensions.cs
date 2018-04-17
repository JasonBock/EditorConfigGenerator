using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class SyntaxNodeExtensions
	{
		internal static BlockSyntax GetPreviousBlock<T>(this SyntaxNode @this)
			where T : SyntaxNode
		{
			var parentStatement = @this.FindParent<T>();
			var parentChildren = parentStatement.ChildNodes().ToArray();
			var nodeIndex = Array.IndexOf(parentChildren, @this);
			var previousNode = parentChildren[nodeIndex - 1];

			if (previousNode is BlockSyntax block)
			{
				return block;
			}
			else
			{
				return previousNode.ChildNodes().Single(_ => _.Kind() == SyntaxKind.Block) as BlockSyntax;
			}
		}

		internal static ExpressionBodiedData Examine(this SyntaxNode @this, ExpressionBodiedData current)
		{
			if (@this == null) { throw new ArgumentNullException(nameof(@this)); }
			if (current == null) { throw new ArgumentNullException(nameof(current)); }

			if (!@this.ContainsDiagnostics)
			{
				var data = current;
				var arrows = @this.ChildNodes().Where(_ => _.Kind() == SyntaxKind.ArrowExpressionClause)
					.Cast<ArrowExpressionClauseSyntax>().ToImmutableArray();

				foreach (var arrow in arrows)
				{
					var lines = arrow.DescendantTrivia().Count(
						_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
					var occurence = lines >= 1 ? ExpressionBodiedDataOccurence.ArrowMultiLine :
						ExpressionBodiedDataOccurence.ArrowSingleLine;
					data = data.Update(occurence);
				}

				var singleLineBlockCount = (uint)@this.ChildNodes().Where(_ => _.Kind() == SyntaxKind.Block)
					.Cast<BlockSyntax>().Count(
						bs => bs.DescendantNodes().Count(_ => typeof(StatementSyntax).IsAssignableFrom(_.GetType())) == 1);

				for (var i = 0; i < singleLineBlockCount; i++)
				{
					data = data.Update(ExpressionBodiedDataOccurence.Block);
				}

				return data;
			}
			else
			{
				return current;
			}
		}

		internal static T FindParent<T>(this SyntaxNode @this)
			where T : SyntaxNode
		{
			var parent = @this.Parent;

			while (!(parent is T) && parent != null)
			{
				parent = parent.Parent;
			}

			return parent as T;
		}
	}
}
