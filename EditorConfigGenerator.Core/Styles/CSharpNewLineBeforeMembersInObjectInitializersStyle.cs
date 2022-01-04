using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles;

public sealed class CSharpNewLineBeforeMembersInObjectInitializersStyle
	: SeverityNodeStyle<BooleanData, InitializerExpressionSyntax, NodeInformation<InitializerExpressionSyntax>, CSharpNewLineBeforeMembersInObjectInitializersStyle>
{
	public const string Setting = "csharp_new_line_before_members_in_object_initializers";

	public CSharpNewLineBeforeMembersInObjectInitializersStyle(BooleanData data, Severity severity = Severity.Error)
		: base(data, severity) { }

	public override CSharpNewLineBeforeMembersInObjectInitializersStyle Add(CSharpNewLineBeforeMembersInObjectInitializersStyle style)
	{
		if (style is null) { throw new ArgumentNullException(nameof(style)); }
		return new CSharpNewLineBeforeMembersInObjectInitializersStyle(this.Data.Add(style.Data), this.Severity);
	}

	public override string GetSetting()
	{
		if (this.Data.TotalOccurences > 0)
		{
			var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
			return $"{CSharpNewLineBeforeMembersInObjectInitializersStyle.Setting} = {value}:{this.Severity.GetDescription()}";
		}
		else
		{
			return string.Empty;
		}
	}

	public override CSharpNewLineBeforeMembersInObjectInitializersStyle Update(NodeInformation<InitializerExpressionSyntax> information)
	{
		if (information is null) { throw new ArgumentNullException(nameof(information)); }

		var node = information.Node;

		if (!node.ContainsDiagnostics)
		{
			var commas = node.ChildTokens().Where(_ => _.Kind() == SyntaxKind.CommaToken).ToArray();

			if (commas.Length > 0)
			{
				return new CSharpNewLineBeforeMembersInObjectInitializersStyle(
					this.Data.Update(commas.Any(_ => _.HasTrailingTrivia && _.TrailingTrivia.Any(t => t.Kind() == SyntaxKind.EndOfLineTrivia))),
						this.Severity);
			}
		}

		return new CSharpNewLineBeforeMembersInObjectInitializersStyle(this.Data, this.Severity);
	}
}