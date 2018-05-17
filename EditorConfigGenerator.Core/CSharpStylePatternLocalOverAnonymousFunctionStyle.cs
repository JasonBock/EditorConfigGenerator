using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core
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

		//// If you have a LocalFunctionStatement, then that's a "true" instance.
		//// Otherwise, look for VariableDeclaration
		//// It can either have an IdentifierName or GenericName
		//// If it's an "Action" or "Func" of any "kind", false
		//public void Foo()
		//{
		//	// csharp_style_pattern_local_over_anonymous_function = true
		//	int fibonacci(int n)
		//	{
		//		return n <= 1 ? 1 : fibonacci(n - 1) + fibonacci(n - 2);
		//	}

		//	Action a = null;
		//	// csharp_style_pattern_local_over_anonymous_function = false
		//	Func<int, int> f2 = (int n) =>
		//	{
		//		return n <= 1 ? 1 : fibonacci(n - 1) + fibonacci(n - 2);
		//	};
		//}
	}
}
