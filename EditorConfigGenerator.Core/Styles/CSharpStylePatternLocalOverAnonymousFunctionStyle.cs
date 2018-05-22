using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStylePatternLocalOverAnonymousFunctionStyle
		: ModelSeverityNodeStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, CSharpStylePatternLocalOverAnonymousFunctionStyle>
	{
		public const string Setting = "csharp_style_pattern_local_over_anonymous_function";

		public CSharpStylePatternLocalOverAnonymousFunctionStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStylePatternLocalOverAnonymousFunctionStyle Add(CSharpStylePatternLocalOverAnonymousFunctionStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStylePatternLocalOverAnonymousFunctionStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpStylePatternLocalOverAnonymousFunctionStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStylePatternLocalOverAnonymousFunctionStyle Update(ModelNodeInformation<SyntaxNode> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if(node is LocalFunctionStatementSyntax function)
				{
					return new CSharpStylePatternLocalOverAnonymousFunctionStyle(this.Data.Update(true), this.Severity);
				}
				else if(node is VariableDeclarationSyntax variable)
				{
					ISymbol variableSymbol = default;

					if (node.ChildNodes().SingleOrDefault(_ => _ is GenericNameSyntax) is GenericNameSyntax genericVariable)
					{
						variableSymbol = model.GetSymbolInfo(genericVariable).Symbol;
					}
					else if (node.ChildNodes().SingleOrDefault(_ => _ is IdentifierNameSyntax) is IdentifierNameSyntax identifierVariable)
					{
						variableSymbol = model.GetSymbolInfo(identifierVariable).Symbol;
					}

					if(variableSymbol is INamedTypeSymbol namedVariableSymbol &&
						namedVariableSymbol.TypeKind == TypeKind.Delegate)
					{
						var original = namedVariableSymbol.OriginalDefinition;
						return new CSharpStylePatternLocalOverAnonymousFunctionStyle(
							this.Data.Update(original.Name.StartsWith("System.Action") ||
								original.Name.StartsWith("System.Func")), this.Severity);
					}
				}
			}

			return new CSharpStylePatternLocalOverAnonymousFunctionStyle(this.Data, this.Severity);
		}
	}
}
