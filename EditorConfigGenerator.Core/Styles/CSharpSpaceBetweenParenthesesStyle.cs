namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceBetweenParenthesesStyle
	{
		public void Foo()
		{
			// ForStatementSyntax, IfStatementSyntax, SwitchStatementSyntax, WhileStatementSyntax, ForEachStatement
			// for, if, switch, while, foreach
			// csharp_space_between_parentheses = control_flow_statements

			// BinaryExpressionSyntax
			// csharp_space_between_parentheses = expressions

			// CastExpressionSyntax
			// csharp_space_between_parentheses = type_casts

			// For ALL of these, it might be the easiest to get the string for the statement/expression,
			// find the first left parenthesis, and last right parenthesis, and see if there's any
			// whitespace immediately after and before.

			// The "better" way would be to check for trivia, but with the way the tree works,
			// it may not be easy.
		}
	}
}
