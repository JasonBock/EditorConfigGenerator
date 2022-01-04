using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles;

[TestFixture]
public static class NodeInformationTests
{
	[Test]
	public static void Create()
	{
		var node = SyntaxFactory.IdentifierName("f");
		var information = new NodeInformation<IdentifierNameSyntax>(node);
		Assert.That(information.Node, Is.SameAs(node));
	}

	[Test]
	public static void CreateWithNull() =>
		Assert.That(() => new NodeInformation<IdentifierNameSyntax>(null!), Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void DoExplicitCast()
	{
		var node = SyntaxFactory.IdentifierName("f");
		NodeInformation<IdentifierNameSyntax> information = node;
		Assert.That(information.Node, Is.SameAs(node));
	}

	[Test]
	public static void DoImplicitCast()
	{
		var node = SyntaxFactory.IdentifierName("f");
		var information = new NodeInformation<IdentifierNameSyntax>(node);
		var newNode = (IdentifierNameSyntax)information;

		Assert.That(newNode, Is.SameAs(node));
	}
}