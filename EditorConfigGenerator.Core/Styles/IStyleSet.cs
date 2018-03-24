namespace EditorConfigGenerator.Core.Styles
{
	public interface IStyleSet
	{
		CSharpStyleExpressionBodiedConstructorsStyle CSharpStyleExpressionBodiedConstructorsStyle { get; }
		CSharpStyleExpressionBodiedMethodsStyle CSharpStyleExpressionBodiedMethodsStyle { get; }
		CSharpStyleExpressionBodiedOperatorsStyle CSharpStyleExpressionBodiedOperatorsStyle { get; }
		CSharpStyleInlinedVariableDeclarationStyle CSharpStyleInlinedVariableDeclarationStyle { get; }
		CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; }
		CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; }
		DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; }
		DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; }
		IndentStyleStyle IndentStyleStyle { get; }

		IStyleSet Update(IStyleSet set);
	}
}