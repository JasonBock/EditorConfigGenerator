﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleInlinedVariableDeclarationStyle
		: SeverityStyle<BooleanData, ArgumentSyntax, CSharpStyleInlinedVariableDeclarationStyle>
	{
		public CSharpStyleInlinedVariableDeclarationStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleInlinedVariableDeclarationStyle Add(CSharpStyleInlinedVariableDeclarationStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleInlinedVariableDeclarationStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_style_inlined_variable_declaration = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleInlinedVariableDeclarationStyle Update(ArgumentSyntax node)
		{
			if(node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				var isOut = node.DescendantTokens().Any(_ => _.RawKind == (int)SyntaxKind.OutKeyword);

				if (isOut)
				{
					return new CSharpStyleInlinedVariableDeclarationStyle(this.Data.Update(
						node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.DeclarationExpression)));
				}
				else
				{
					return new CSharpStyleInlinedVariableDeclarationStyle(this.Data);
				}
			}
			else
			{
				return new CSharpStyleInlinedVariableDeclarationStyle(this.Data);
			}
		}
	}
}
