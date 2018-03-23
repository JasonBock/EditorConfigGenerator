using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class SyntaxNodeExtensions
	{
		internal static T FindParent<T>(this SyntaxNode @this)
			where T : SyntaxNode
		{
			var parent = @this.Parent;

			while (!(parent is T))
			{
				parent = parent.Parent;
			}

			return parent as T;
		}
	}
}
