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
		DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; }
		IndentStyleStyle IndentStyleStyle { get; }

		IStyleSet Update(IStyleSet set);
	}
}