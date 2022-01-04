using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class TokenInformationTests
{
	[Test]
	public static void Create()
	{
		var token = SyntaxFactory.Token(SyntaxKind.AmpersandToken);
		var information = new TokenInformation(token);
		Assert.That(information.Token, Is.EqualTo(token));
	}

	[Test]
	public static void DoExplicitCast()
	{
		var token = SyntaxFactory.Token(SyntaxKind.AmpersandToken);
		TokenInformation information = token;
		Assert.That(information.Token, Is.EqualTo(token));
	}

	[Test]
	public static void DoImplicitCast()
	{
		var token = SyntaxFactory.Token(SyntaxKind.AmpersandToken);
		var information = new TokenInformation(token);
		var newNode = (SyntaxToken)information;

		Assert.That(newNode, Is.EqualTo(token));
	}
}