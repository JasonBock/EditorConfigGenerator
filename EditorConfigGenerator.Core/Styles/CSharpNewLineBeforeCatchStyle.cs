﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpNewLineBeforeCatchStyle
	: SeverityNodeStyle<BooleanData, CatchClauseSyntax, NodeInformation<CatchClauseSyntax>, CSharpNewLineBeforeCatchStyle>
{
	public const string Setting = "csharp_new_line_before_catch";

	public CSharpNewLineBeforeCatchStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpNewLineBeforeCatchStyle Add(CSharpNewLineBeforeCatchStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpNewLineBeforeCatchStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{CSharpNewLineBeforeCatchStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override CSharpNewLineBeforeCatchStyle Update(NodeInformation<CatchClauseSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			var parentStatement = node.FindParent<TryStatementSyntax>();

			if (parentStatement is not null)
			{
				var parentChildren = parentStatement.ChildNodes().ToArray();
				var nodeIndex = Array.IndexOf(parentChildren, node);
				var previousNode = parentChildren[nodeIndex - 1];

				if (previousNode is not BlockSyntax block)
				{
					block = (BlockSyntax)((CatchClauseSyntax)previousNode).ChildNodes().Last();
				}

				var closeToken = block.ChildTokens().Single(_ => _.RawKind == (int)SyntaxKind.CloseBraceToken);
				var containsEol = closeToken.HasTrailingTrivia &&
					closeToken.TrailingTrivia.Any(_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
				return new CSharpNewLineBeforeCatchStyle(this.Data.Update(containsEol), this.Severity);
			}
		}

		return new CSharpNewLineBeforeCatchStyle(this.Data, this.Severity);
	}
}