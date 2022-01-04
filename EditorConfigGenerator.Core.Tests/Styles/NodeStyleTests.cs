using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace EditorConfigGenerator.Core.Tests.Styles
{
   [TestFixture]
	public static class NodeStyleTests
	{
		[Test]
		public static void Create()
		{
			var data = new BooleanData();
			var style = new TestStyle(data);
			Assert.That(style.Data, Is.SameAs(data));
		}

		[Test]
		public static void CreateWhenDataIsNull() =>
			Assert.That(() => new TestStyle(null!), Throws.TypeOf<ArgumentNullException>());

		private sealed class TestStyle
			: NodeStyle<BooleanData, SyntaxNode, NodeInformation<SyntaxNode>, TestStyle>
		{
			public TestStyle(BooleanData data) 
				: base(data) { }

			public override TestStyle Add(TestStyle style) => 
				throw new NotImplementedException();

			public override string GetSetting() => 
				throw new NotImplementedException();

			public override TestStyle Update(NodeInformation<SyntaxNode> information) => 
				throw new NotImplementedException();
		}
	}
}
