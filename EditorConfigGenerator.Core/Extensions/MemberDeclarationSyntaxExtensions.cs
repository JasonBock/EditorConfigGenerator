using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class MemberDeclarationSyntaxExtensions
	{
		internal static ExpressionBodiedData Examine(this MemberDeclarationSyntax @this, ExpressionBodiedData current)
		{
			if (@this == null) { throw new ArgumentNullException(nameof(@this)); }
			if (current == null) { throw new ArgumentNullException(nameof(current)); }

			if (!@this.ContainsDiagnostics)
			{
				var arrowExpression = @this.DescendantNodes().SingleOrDefault(
					_ => _.Kind() == SyntaxKind.ArrowExpressionClause);

				if (arrowExpression != null)
				{
					var lines = arrowExpression.DescendantTrivia().Count(
						_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
					var occurence = lines >= 1 ? ExpressionBodiedDataOccurence.ArrowMultiLine : 
						ExpressionBodiedDataOccurence.ArrowSingleLine;
					return current.Update(occurence);
				}
				else
				{
					var statementSyntaxCount = @this.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.Block)
						?.DescendantNodes().Count(_ => typeof(StatementSyntax).IsAssignableFrom(_.GetType()));

					if (statementSyntaxCount > 1)
					{
						return current;
					}
					else if (statementSyntaxCount == 1)
					{
						var nodes = @this.DescendantTokens().ToImmutableArray();

						var lines = 0;

						for(var i = 0; i < nodes.Length - 1; i++)
						{
							lines += nodes[i].GetAllTrivia().Count(
								_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
						}

						var occurence = lines >= 1 ? ExpressionBodiedDataOccurence.BlockMultiLine :
							ExpressionBodiedDataOccurence.BlockSingleLine;
						return current.Update(occurence);
					}
					else
					{
						return current;
					}
				}
			}
			else
			{
				return current;
			}
		}
	}
}
