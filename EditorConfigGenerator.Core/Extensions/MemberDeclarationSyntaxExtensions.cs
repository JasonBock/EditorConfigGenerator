using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class MemberDeclarationSyntaxExtensions
	{
		internal static BooleanData Examine(this MemberDeclarationSyntax @this, BooleanData current)
		{
			if (@this == null) { throw new ArgumentNullException(nameof(@this)); }
			if (current == null) { throw new ArgumentNullException(nameof(current)); }

			if (!@this.ContainsDiagnostics)
			{
				var arrowExpressionExists = @this.DescendantNodes().Any(
					_ => _.Kind() == SyntaxKind.ArrowExpressionClause);

				if (arrowExpressionExists)
				{
					return current.Update(true);
				}
				else
				{
					var statementSyntaxCount = @this.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.Block)
						?.DescendantNodes().Count(_ => typeof(StatementSyntax).IsAssignableFrom(_.GetType()));

					return statementSyntaxCount == 1 ?
						current.Update(false) : current;
				}
			}
			else
			{
				return current;
			}
		}
	}
}
