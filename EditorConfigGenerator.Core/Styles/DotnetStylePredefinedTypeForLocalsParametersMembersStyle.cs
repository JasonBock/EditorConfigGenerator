using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SpecialNameExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStylePredefinedTypeForLocalsParametersMembersStyle
		: ModelSeverityStyle<BooleanData, SyntaxNode, ModelNodeInformation<SyntaxNode>, DotnetStylePredefinedTypeForLocalsParametersMembersStyle>
	{
		public DotnetStylePredefinedTypeForLocalsParametersMembersStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStylePredefinedTypeForLocalsParametersMembersStyle Add(DotnetStylePredefinedTypeForLocalsParametersMembersStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_style_predefined_type_for_locals_parameters_members = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStylePredefinedTypeForLocalsParametersMembersStyle Update(ModelNodeInformation<SyntaxNode> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if (node is ParameterSyntax || node is LocalDeclarationStatementSyntax ||
					node is FieldDeclarationSyntax || node is PropertyDeclarationSyntax)
				{
					if (node.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.PredefinedType))
					{
						return new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(
							this.Data.Update(true), this.Severity);
					}
					else
					{
						if (node.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName) is IdentifierNameSyntax identifierNode)
						{
							if (model.GetSymbolInfo(identifierNode).Symbol is ITypeSymbol identifierType && 
								identifierType.SpecialType.IsPredefinedType())
							{
								return new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(
									this.Data.Update(false), this.Severity);
							}
						}
					}
				}
			}

			return new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(this.Data, this.Severity);
		}
	}
}