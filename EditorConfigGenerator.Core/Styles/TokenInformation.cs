using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Styles;

public class TokenInformation
{
	public TokenInformation(SyntaxToken token) => this.Token = token;

	public static implicit operator TokenInformation(SyntaxToken token) => new TokenInformation(token);

	public static explicit operator SyntaxToken(TokenInformation information)
	{
		if (information is null)
		{
			throw new ArgumentNullException(nameof(information));
		}

		return information.Token;
	}

	public SyntaxToken Token { get; }
}