namespace EditorConfigGenerator.Core.Styles
{
	public interface IStyleSet
	{
		CSharpNewLineBeforeCatchStyle CSharpNewLineBeforeCatchStyle { get; }
		CSharpNewLineBeforeElseStyle CSharpNewLineBeforeElseStyle { get; }
		CSharpNewLineBeforeFinallyStyle CSharpNewLineBeforeFinallyStyle { get; }
		CSharpPreferBracesStyle CSharpPreferBracesStyle { get; }
		CSharpPreferredModifierOrderStyle CSharpPreferredModifierOrderStyle { get; }
		CSharpPreferSimpleDefaultExpressionStyle CSharpPreferSimpleDefaultExpressionStyle { get; }
		CSharpSpaceAfterCastStyle CSharpSpaceAfterCastStyle { get; }
		CSharpSpaceAfterKeywordsInControlFlowStatementsStyle CSharpSpaceAfterKeywordsInControlFlowStatementsStyle { get; }
		CSharpSpaceBetweenMethodCallParameterListParenthesesStyle CSharpSpaceBetweenMethodCallParameterListParenthesesStyle { get; }
		CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle { get; }
		CSharpSpaceBetweenParenthesesStyle CSharpSpaceBetweenParenthesesStyle { get; }
		CSharpStyleExpressionBodiedAccessorsStyle CSharpStyleExpressionBodiedAccessorsStyle { get; }
		CSharpStyleExpressionBodiedConstructorsStyle CSharpStyleExpressionBodiedConstructorsStyle { get; }
		CSharpStyleExpressionBodiedIndexersStyle CSharpStyleExpressionBodiedIndexersStyle { get; }
		CSharpStyleExpressionBodiedMethodsStyle CSharpStyleExpressionBodiedMethodsStyle { get; }
		CSharpStyleExpressionBodiedOperatorsStyle CSharpStyleExpressionBodiedOperatorsStyle { get; }
		CSharpStyleExpressionBodiedPropertiesStyle CSharpStyleExpressionBodiedPropertiesStyle { get; }
		CSharpStyleInlinedVariableDeclarationStyle CSharpStyleInlinedVariableDeclarationStyle { get; }
		CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; }
		CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; }
		DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; }
		DotnetStyleExplicitTupleNamesStyle DotnetStyleExplicitTupleNamesStyle { get; }
		DotnetStylePredefinedTypeForLocalsParametersMembersStyle DotnetStylePredefinedTypeForLocalsParametersMembersStyle { get; }
		DotnetStylePredefinedTypeForMemberAccessStyle DotnetStylePredefinedTypeForMemberAccessStyle { get; }
		DotnetStylePreferInferredAnonymousTypeMemberNamesStyle DotnetStylePreferInferredAnonymousTypeMemberNamesStyle { get; }
		DotnetStylePreferInferredTupleNamesStyle DotnetStylePreferInferredTupleNamesStyle { get; }
		DotnetStyleQualificationForEventStyle DotnetStyleQualificationForEventStyle { get; }
		DotnetStyleQualificationForFieldStyle DotnetStyleQualificationForFieldStyle { get; }
		DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; }
		DotnetStyleQualificationForPropertyStyle DotnetStyleQualificationForPropertyStyle { get; }
		DotnetStyleRequireAccessibilityModifiersStyle DotnetStyleRequireAccessibilityModifiersStyle { get; }
		IndentStyleStyle IndentStyleStyle { get; }

		IStyleSet Update(IStyleSet set);
	}
}