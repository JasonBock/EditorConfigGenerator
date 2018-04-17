using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpNewLineBeforeElseStyle
		: SeverityStyle<BooleanData, ElseClauseSyntax, NodeInformation<ElseClauseSyntax>, CSharpNewLineBeforeElseStyle>
	{
		public CSharpNewLineBeforeElseStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpNewLineBeforeElseStyle Add(CSharpNewLineBeforeElseStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpNewLineBeforeElseStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{		
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_new_line_before_else = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpNewLineBeforeElseStyle Update(NodeInformation<ElseClauseSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var block = node.GetPreviousBlock<IfStatementSyntax>();
				var closeToken = block.ChildTokens().Single(_ => _.RawKind == (int)SyntaxKind.CloseBraceToken);
				var containsEol = closeToken.HasTrailingTrivia && 
					closeToken.TrailingTrivia.Any(_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
				return new CSharpNewLineBeforeElseStyle(this.Data.Update(containsEol), this.Severity);
			}

			return new CSharpNewLineBeforeElseStyle(this.Data, this.Severity);
		}
	}
}