using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles
{
	public class TokenInformation
	{
		public TokenInformation(SyntaxToken token) => this.Token = token;

		public static implicit operator TokenInformation(SyntaxToken token) => new TokenInformation(token);

		public static explicit operator SyntaxToken(TokenInformation information) => information.Token;

		public SyntaxToken Token { get; }
	}
}
