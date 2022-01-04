namespace EditorConfigGenerator.Core.Styles;

public interface IStyleSet
{
	CSharpNewLineBeforeCatchStyle CSharpNewLineBeforeCatchStyle { get; }
	CSharpNewLineBeforeElseStyle CSharpNewLineBeforeElseStyle { get; }
	CSharpNewLineBeforeMembersInAnonymousTypesStyle CSharpNewLineBeforeMembersInAnonymousTypesStyle { get; }
	CSharpNewLineBeforeMembersInObjectInitializersStyle CSharpNewLineBeforeMembersInObjectInitializersStyle { get; }
	CSharpNewLineBeforeFinallyStyle CSharpNewLineBeforeFinallyStyle { get; }
	CSharpNewLineBetweenQueryExpressionClausesStyle CSharpNewLineBetweenQueryExpressionClausesStyle { get; }
	CSharpPreferBracesStyle CSharpPreferBracesStyle { get; }
	CSharpPreferredModifierOrderStyle CSharpPreferredModifierOrderStyle { get; }
	CSharpPreferSimpleDefaultExpressionStyle CSharpPreferSimpleDefaultExpressionStyle { get; }
	CSharpPreserveSingleLineBlocksStyle CSharpPreserveSingleLineBlocksStyle { get; }
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
	CSharpStylePatternLocalOverAnonymousFunctionStyle CSharpStylePatternLocalOverAnonymousFunctionStyle { get; }
	CSharpStylePatternMatchingOverAsWithNullCheckStyle CSharpStylePatternMatchingOverAsWithNullCheckStyle { get; }
	CSharpStyleVarElsewhereStyle CSharpStyleVarElsewhereStyle { get; }
	CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; }
	CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; }
	DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; }
	DotnetStyleExplicitTupleNamesStyle DotnetStyleExplicitTupleNamesStyle { get; }
	DotnetStyleObjectInitializerStyle DotnetStyleObjectInitializerStyle { get; }
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

	IStyleSet Update(IStyleSet styleSet);
}