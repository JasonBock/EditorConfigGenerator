using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleInlinedVariableDeclarationStyle
		: SeverityNodeStyle<BooleanData, ArgumentSyntax, NodeInformation<ArgumentSyntax>, CSharpStyleInlinedVariableDeclarationStyle>
	{
		public const string Setting = "csharp_style_inlined_variable_declaration";

		public CSharpStyleInlinedVariableDeclarationStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleInlinedVariableDeclarationStyle Add(CSharpStyleInlinedVariableDeclarationStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleInlinedVariableDeclarationStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpStyleInlinedVariableDeclarationStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleInlinedVariableDeclarationStyle Update(NodeInformation<ArgumentSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var isOut = node.DescendantTokens().Any(_ => _.RawKind == (int)SyntaxKind.OutKeyword);

				if (isOut)
				{
					return new CSharpStyleInlinedVariableDeclarationStyle(this.Data.Update(
						node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.DeclarationExpression)), this.Severity);
				}
			}

			return new CSharpStyleInlinedVariableDeclarationStyle(this.Data, this.Severity);
		}
	}
}