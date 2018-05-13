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
		: SeverityNodeStyle<BooleanData, ElseClauseSyntax, NodeInformation<ElseClauseSyntax>, CSharpNewLineBeforeElseStyle>
	{
		public const string Setting = "csharp_new_line_before_else";

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
				return $"{CSharpNewLineBeforeElseStyle.Setting} = {value}:{this.Severity.GetDescription()}";
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
				var parentStatement = node.FindParent<IfStatementSyntax>();
				var parentChildren = parentStatement.ChildNodes().ToArray();
				var nodeIndex = Array.IndexOf(parentChildren, node);
				var previousNode = parentChildren[nodeIndex - 1];

				var lastToken = previousNode.ChildTokens().Last();
				var containsEol = lastToken.HasTrailingTrivia && 
					lastToken.TrailingTrivia.Any(_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
				return new CSharpNewLineBeforeElseStyle(this.Data.Update(containsEol), this.Severity);
			}

			return new CSharpNewLineBeforeElseStyle(this.Data, this.Severity);
		}
	}
}