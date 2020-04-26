using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceAfterKeywordsInControlFlowStatementsStyle
		: SeverityTokenStyle<BooleanData, TokenInformation, CSharpSpaceAfterKeywordsInControlFlowStatementsStyle>
	{
		public const string Setting = "csharp_space_after_keywords_in_control_flow_statements";

		public CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpSpaceAfterKeywordsInControlFlowStatementsStyle Add(CSharpSpaceAfterKeywordsInControlFlowStatementsStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpSpaceAfterKeywordsInControlFlowStatementsStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpSpaceAfterKeywordsInControlFlowStatementsStyle Update(TokenInformation information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var token = information.Token;

			if (!token.Parent?.ContainsDiagnostics ?? false)
			{
				var kind = (SyntaxKind)token.RawKind;

				if(kind == SyntaxKind.ForKeyword || kind == SyntaxKind.ForEachKeyword ||
					kind == SyntaxKind.IfKeyword || kind == SyntaxKind.SwitchKeyword ||
					kind == SyntaxKind.WhileKeyword)
				{
					return new CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(
						this.Data.Update(token.HasTrailingTrivia && 
							token.TrailingTrivia.Any(_ => _.Kind() == SyntaxKind.WhitespaceTrivia)));
				}
				else
				{
					return new CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(this.Data, this.Severity);
				}
			}

			return new CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(this.Data, this.Severity);
		}
	}
}
