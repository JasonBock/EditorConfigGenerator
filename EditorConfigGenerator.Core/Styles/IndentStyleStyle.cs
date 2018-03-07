using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class IndentStyleStyle
		: Style<TabSpaceData, SyntaxNode, IndentStyleStyle>
	{
		public IndentStyleStyle(TabSpaceData data)
			: base(data) { }

		public override IndentStyleStyle Add(IndentStyleStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new IndentStyleStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TabOccurences >= this.Data.SpaceOccurences ? "tab" : "space";
				return $"indent_style = {value}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override IndentStyleStyle Update(SyntaxNode node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			var leadingTrivia = node.GetLeadingTrivia().Where(_ => _.IsKind(SyntaxKind.WhitespaceTrivia)).ToArray();

			if(leadingTrivia.Length == 1)
			{
				var content = leadingTrivia[0].ToFullString();
				return new IndentStyleStyle(
					this.Data.Update(content.Contains("\t")));
			}
			else
			{
				return new IndentStyleStyle(this.Data);
			}
		}
	}
}