namespace EditorConfigGenerator.Core.Styles
{
	public interface IStyleSet
	{
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
		DotnetStyleQualificationForEventStyle DotnetStyleQualificationForEventStyle { get; }
		DotnetStyleQualificationForFieldStyle DotnetStyleQualificationForFieldStyle { get; }
		DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; }
		DotnetStyleQualificationForPropertyStyle DotnetStyleQualificationForPropertyStyle { get; }
		IndentStyleStyle IndentStyleStyle { get; }

		IStyleSet Update(IStyleSet set);
	}
}