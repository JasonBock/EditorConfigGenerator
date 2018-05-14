using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class IndentStyleStyle
		: NodeStyle<TabSpaceData, SyntaxNode, NodeInformation<SyntaxNode>, IndentStyleStyle>
	{
		public const string Setting = "indent_style";

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
				return $"{IndentStyleStyle.Setting} = {value}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override IndentStyleStyle Update(NodeInformation<SyntaxNode> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			var leadingTrivia = node.GetLeadingTrivia().Where(_ => _.IsKind(SyntaxKind.WhitespaceTrivia)).ToArray();

			if (leadingTrivia.Length == 1)
			{
				var content = leadingTrivia[0].ToFullString();
				return new IndentStyleStyle(this.Data.Update(content.Contains("\t")));
			}

			return new IndentStyleStyle(this.Data);
		}
	}
}