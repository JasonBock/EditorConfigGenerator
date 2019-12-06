using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpPreferBracesStyle
		: SeverityNodeStyle<BooleanData, SyntaxNode, NodeInformation<SyntaxNode>, CSharpPreferBracesStyle>
	{
		public const string Setting = "csharp_prefer_braces";

		public CSharpPreferBracesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpPreferBracesStyle Add(CSharpPreferBracesStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpPreferBracesStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpPreferBracesStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpPreferBracesStyle Update(NodeInformation<SyntaxNode> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				if(node is IfStatementSyntax || node is ElseClauseSyntax ||
					node is ForStatementSyntax || node is ForEachStatementSyntax ||
					node is WhileStatementSyntax)
				{
					if (node.ChildNodes().SingleOrDefault(_ => _.Kind() == SyntaxKind.Block) is BlockSyntax block)
					{
						return block.ChildNodes().Count() == 1 ?
							new CSharpPreferBracesStyle(this.Data.Update(true), this.Severity) :
							new CSharpPreferBracesStyle(this.Data, this.Severity);
					}
					else
					{
						return new CSharpPreferBracesStyle(this.Data.Update(false), this.Severity);
					}
				}
			}

			return new CSharpPreferBracesStyle(this.Data, this.Severity);
		}
	}
}
