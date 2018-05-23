using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class TokenStyleTests
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
			Assert.That(() => new TestStyle(null), Throws.TypeOf<ArgumentNullException>());

		private sealed class TestStyle
			: TokenStyle<BooleanData, TokenInformation, TestStyle>
		{
			public TestStyle(BooleanData data) 
				: base(data) { }

			public override TestStyle Add(TestStyle style) => 
				throw new NotImplementedException();

			public override string GetSetting() => 
				throw new NotImplementedException();

			public override TestStyle Update(TokenInformation information) => 
				throw new NotImplementedException();
		}
	}
}
