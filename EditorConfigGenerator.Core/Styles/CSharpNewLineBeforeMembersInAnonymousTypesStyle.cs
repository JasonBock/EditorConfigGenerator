using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpNewLineBeforeMembersInAnonymousTypesStyle
		: SeverityNodeStyle<BooleanData, AnonymousObjectCreationExpressionSyntax, NodeInformation<AnonymousObjectCreationExpressionSyntax>, CSharpNewLineBeforeMembersInAnonymousTypesStyle>
	{
		public CSharpNewLineBeforeMembersInAnonymousTypesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpNewLineBeforeMembersInAnonymousTypesStyle Add(CSharpNewLineBeforeMembersInAnonymousTypesStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpNewLineBeforeMembersInAnonymousTypesStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_new_line_before_members_in_anonymous_types = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpNewLineBeforeMembersInAnonymousTypesStyle Update(NodeInformation<AnonymousObjectCreationExpressionSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var commas = node.ChildTokens().Where(_ => _.Kind() == SyntaxKind.CommaToken).ToArray();

				if (commas.Length > 0)
				{
					return new CSharpNewLineBeforeMembersInAnonymousTypesStyle(
						this.Data.Update(commas.Any(_ => _.HasTrailingTrivia && _.TrailingTrivia.Any(t => t.Kind() == SyntaxKind.EndOfLineTrivia))),
							this.Severity);
				}
			}

			return new CSharpNewLineBeforeMembersInAnonymousTypesStyle(this.Data, this.Severity);
		}
	}
}
