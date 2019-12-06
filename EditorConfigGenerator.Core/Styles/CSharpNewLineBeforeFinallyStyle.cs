using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpNewLineBeforeFinallyStyle
		: SeverityNodeStyle<BooleanData, FinallyClauseSyntax, NodeInformation<FinallyClauseSyntax>, CSharpNewLineBeforeFinallyStyle>
	{
		public const string Setting = "csharp_new_line_before_finally";

		public CSharpNewLineBeforeFinallyStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpNewLineBeforeFinallyStyle Add(CSharpNewLineBeforeFinallyStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpNewLineBeforeFinallyStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpNewLineBeforeFinallyStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpNewLineBeforeFinallyStyle Update(NodeInformation<FinallyClauseSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var parentStatement = node.FindParent<TryStatementSyntax>();
				var parentChildren = parentStatement.ChildNodes().ToArray();
				var nodeIndex = Array.IndexOf(parentChildren, node);
				var previousNode = parentChildren[nodeIndex - 1];

				if (!(previousNode is BlockSyntax block))
				{
					block = (BlockSyntax)((CatchClauseSyntax)previousNode).ChildNodes().Last();
				}

				var closeToken = block.ChildTokens().Single(_ => _.RawKind == (int)SyntaxKind.CloseBraceToken);
				var containsEol = closeToken.HasTrailingTrivia && 
					closeToken.TrailingTrivia.Any(_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
				return new CSharpNewLineBeforeFinallyStyle(this.Data.Update(containsEol), this.Severity);
			}

			return new CSharpNewLineBeforeFinallyStyle(this.Data, this.Severity);
		}
	}
}