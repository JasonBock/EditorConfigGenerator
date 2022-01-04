using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpPreserveSingleLineBlocksStyle
	: SeverityNodeStyle<BooleanData, AccessorListSyntax, NodeInformation<AccessorListSyntax>, CSharpPreserveSingleLineBlocksStyle>
{
	public const string Setting = "csharp_preserve_single_line_blocks";

	public CSharpPreserveSingleLineBlocksStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpPreserveSingleLineBlocksStyle Add(CSharpPreserveSingleLineBlocksStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpPreserveSingleLineBlocksStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{CSharpPreserveSingleLineBlocksStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override CSharpPreserveSingleLineBlocksStyle Update(NodeInformation<AccessorListSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			var openBrace = node.ChildTokens().First(_ => _.IsKind(SyntaxKind.OpenBraceToken));

			return new CSharpPreserveSingleLineBlocksStyle(
				this.Data.Update(!(openBrace.HasTrailingTrivia &&
					openBrace.TrailingTrivia.Any(_ => _.IsKind(SyntaxKind.EndOfLineTrivia)))), this.Severity);
		}

		return new CSharpPreserveSingleLineBlocksStyle(this.Data, this.Severity);
	}
}