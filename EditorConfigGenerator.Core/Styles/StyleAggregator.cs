using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class StyleAggregator
	{
		public StyleAggregator Update(StyleAggregator aggregator) =>
			new StyleAggregator() { Set = this.Set.Update(aggregator.Set) };

		public StyleAggregator Add(CompilationUnitSyntax syntax, SemanticModel model)
		{
			var walker = new StyleWalker(model);
			walker.VisitCompilationUnit(syntax);
			return new StyleAggregator() { Set = this.Set.Update(walker.Set) };
		}

		public string GenerateConfiguration()
		{
			void AppendSetting(string setting, StringBuilder appendBuilder)
			{
				if (!string.IsNullOrWhiteSpace(setting)) { appendBuilder.AppendLine(setting); }
			}

			var builder = new StringBuilder();
			builder.AppendLine("[*.cs]");
			AppendSetting(this.Set.CSharpNewLineBeforeCatchStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpNewLineBeforeElseStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpNewLineBeforeFinallyStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpNewLineBeforeMembersInAnonymousTypesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpNewLineBeforeMembersInObjectInitializersStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpNewLineBetweenQueryExpressionClausesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpPreferBracesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpPreferredModifierOrderStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpPreferSimpleDefaultExpressionStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpPreserveSingleLineBlocksStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpSpaceAfterCastStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpSpaceBetweenParenthesesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedAccessorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedConstructorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedIndexersStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedMethodsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedOperatorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedPropertiesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleInlinedVariableDeclarationStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStylePatternLocalOverAnonymousFunctionStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarForBuiltInTypesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarWhenTypeIsApparentStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetSortSystemDirectivesFirstStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleExplicitTupleNamesStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleObjectInitializerStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStylePredefinedTypeForLocalsParametersMembersStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStylePredefinedTypeForMemberAccessStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStylePreferInferredTupleNamesStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForEventStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForFieldStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForMethodStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForPropertyStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleRequireAccessibilityModifiersStyle.GetSetting(), builder);
			AppendSetting(this.Set.IndentStyleStyle.GetSetting(), builder);
			return builder.ToString();
		}

		public IStyleSet Set { get; private set; } = new StyleSet();

		private sealed class StyleWalker
			: CSharpSyntaxWalker
		{
			private readonly SemanticModel model;

			internal StyleWalker(SemanticModel model)
				: base(SyntaxWalkerDepth.Trivia) => this.model = model ?? throw new ArgumentNullException(nameof(model));

			public override void Visit(SyntaxNode node)
			{
				this.Set.CSharpPreferBracesStyle =
					this.Set.CSharpPreferBracesStyle.Update(node);

				if (node is MemberDeclarationSyntax member)
				{
					this.Set.CSharpPreferredModifierOrderStyle =
						this.Set.CSharpPreferredModifierOrderStyle.Update(member);
				}

				this.Set.CSharpSpaceBetweenParenthesesStyle =
					this.Set.CSharpSpaceBetweenParenthesesStyle.Update(
						new NodeInformation<SyntaxNode>(node));
				this.Set.CSharpStylePatternLocalOverAnonymousFunctionStyle =
					this.Set.CSharpStylePatternLocalOverAnonymousFunctionStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStyleQualificationForEventStyle =
					this.Set.DotnetStyleQualificationForEventStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStyleQualificationForFieldStyle =
					this.Set.DotnetStyleQualificationForFieldStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStylePredefinedTypeForLocalsParametersMembersStyle =
					this.Set.DotnetStylePredefinedTypeForLocalsParametersMembersStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStyleQualificationForPropertyStyle =
					this.Set.DotnetStyleQualificationForPropertyStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.IndentStyleStyle = 
					this.Set.IndentStyleStyle.Update(node);
				base.Visit(node);
			}

			public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedAccessorsStyle =
					this.Set.CSharpStyleExpressionBodiedAccessorsStyle.Update(node);
				base.VisitAccessorDeclaration(node);
			}

			public override void VisitAccessorList(AccessorListSyntax node)
			{
				this.Set.CSharpPreserveSingleLineBlocksStyle =
					this.Set.CSharpPreserveSingleLineBlocksStyle.Update(node);
				base.VisitAccessorList(node);
			}

			public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
			{
				this.Set.CSharpNewLineBeforeMembersInAnonymousTypesStyle =
					this.Set.CSharpNewLineBeforeMembersInAnonymousTypesStyle.Update(node);
				base.VisitAnonymousObjectCreationExpression(node);
			}

			public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
			{
				this.Set.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle =
					this.Set.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle.Update(node);
				base.VisitAnonymousObjectMemberDeclarator(node);
			}

			public override void VisitArgument(ArgumentSyntax node)
			{
				this.Set.CSharpStyleInlinedVariableDeclarationStyle =
					this.Set.CSharpStyleInlinedVariableDeclarationStyle.Update(node);
				base.VisitArgument(node);
			}

			public override void VisitArgumentList(ArgumentListSyntax node)
			{
				this.Set.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle =
					this.Set.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.Update(node);
				base.VisitArgumentList(node);
			}

			public override void VisitCastExpression(CastExpressionSyntax node)
			{
				this.Set.CSharpSpaceAfterCastStyle =
					this.Set.CSharpSpaceAfterCastStyle.Update(node);
				base.VisitCastExpression(node);
			}

			public override void VisitCatchClause(CatchClauseSyntax node)
			{
				this.Set.CSharpNewLineBeforeCatchStyle =
					this.Set.CSharpNewLineBeforeCatchStyle.Update(
						new NodeInformation<CatchClauseSyntax>(node));
				base.VisitCatchClause(node);
			}

			public override void VisitClassDeclaration(ClassDeclarationSyntax node)
			{
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitClassDeclaration(node);
			}

			public override void VisitCompilationUnit(CompilationUnitSyntax node)
			{
				this.Set.DotnetSortSystemDirectivesFirstStyle =
					this.Set.DotnetSortSystemDirectivesFirstStyle.Update(node);
				base.VisitCompilationUnit(node);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedConstructorsStyle =
					this.Set.CSharpStyleExpressionBodiedConstructorsStyle.Update(node);
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitConstructorDeclaration(node);
			}

			public override void VisitDefaultExpression(DefaultExpressionSyntax node)
			{
				this.Set.CSharpPreferSimpleDefaultExpressionStyle =
					this.Set.CSharpPreferSimpleDefaultExpressionStyle.Update(
						new NodeInformation<ExpressionSyntax>(node));
				base.VisitDefaultExpression(node);
			}

			public override void VisitElseClause(ElseClauseSyntax node)
			{
				this.Set.CSharpNewLineBeforeElseStyle =
					this.Set.CSharpNewLineBeforeElseStyle.Update(node);
				base.VisitElseClause(node);
			}

			public override void VisitEventDeclaration(EventDeclarationSyntax node)
			{
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitEventDeclaration(node);
			}

			public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
			{
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitFieldDeclaration(node);
			}

			public override void VisitFinallyClause(FinallyClauseSyntax node)
			{
				this.Set.CSharpNewLineBeforeFinallyStyle =
					this.Set.CSharpNewLineBeforeFinallyStyle.Update(
						new NodeInformation<FinallyClauseSyntax>(node));
				base.VisitFinallyClause(node);
			}

			public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedIndexersStyle =
					this.Set.CSharpStyleExpressionBodiedIndexersStyle.Update(node);
				base.VisitIndexerDeclaration(node);
			}

			public override void VisitInitializerExpression(InitializerExpressionSyntax node)
			{
				this.Set.CSharpNewLineBeforeMembersInObjectInitializersStyle =
					this.Set.CSharpNewLineBeforeMembersInObjectInitializersStyle.Update(node);
				base.VisitInitializerExpression(node);
			}

			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				this.Set.DotnetStyleQualificationForMethodStyle =
					this.Set.DotnetStyleQualificationForMethodStyle.Update(
						new ModelNodeInformation<InvocationExpressionSyntax>(node, this.model));
				base.VisitInvocationExpression(node);
			}

			public override void VisitLiteralExpression(LiteralExpressionSyntax node)
			{
				this.Set.CSharpPreferSimpleDefaultExpressionStyle =
					this.Set.CSharpPreferSimpleDefaultExpressionStyle.Update(
						new NodeInformation<ExpressionSyntax>(node));
				base.VisitLiteralExpression(node);
			}

			public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
			{
				this.Set.CSharpStyleVarForBuiltInTypesStyle =
					this.Set.CSharpStyleVarForBuiltInTypesStyle.Update(node);
				this.Set.CSharpStyleVarWhenTypeIsApparentStyle =
					this.Set.CSharpStyleVarWhenTypeIsApparentStyle.Update(node);
				base.VisitLocalDeclarationStatement(node);
			}

			public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
			{
				this.Set.DotnetStyleExplicitTupleNamesStyle =
					this.Set.DotnetStyleExplicitTupleNamesStyle.Update(
						new ModelNodeInformation<MemberAccessExpressionSyntax>(node, this.model));
				this.Set.DotnetStylePredefinedTypeForMemberAccessStyle =
					this.Set.DotnetStylePredefinedTypeForMemberAccessStyle.Update(
						new ModelNodeInformation<MemberAccessExpressionSyntax>(node, this.model));
				base.VisitMemberAccessExpression(node);
			}

			public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedMethodsStyle =
					this.Set.CSharpStyleExpressionBodiedMethodsStyle.Update(node);
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitMethodDeclaration(node);
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
				this.Set.DotnetStyleObjectInitializerStyle =
					this.Set.DotnetStyleObjectInitializerStyle.Update(
						new ModelNodeInformation<ObjectCreationExpressionSyntax>(node, this.model));
				base.VisitObjectCreationExpression(node);
			}

			public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedOperatorsStyle =
					this.Set.CSharpStyleExpressionBodiedOperatorsStyle.Update(node);
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitOperatorDeclaration(node);
			}

			public override void VisitParameterList(ParameterListSyntax node)
			{
				this.Set.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle =
					this.Set.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle.Update(node);
				base.VisitParameterList(node);
			}

			public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedPropertiesStyle =
					this.Set.CSharpStyleExpressionBodiedPropertiesStyle.Update(node);
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitPropertyDeclaration(node);
			}

			public override void VisitQueryExpression(QueryExpressionSyntax node)
			{
				this.Set.CSharpNewLineBetweenQueryExpressionClausesStyle =
					this.Set.CSharpNewLineBetweenQueryExpressionClausesStyle.Update(node);
				base.VisitQueryExpression(node);
			}

			public override void VisitStructDeclaration(StructDeclarationSyntax node)
			{
				this.Set.DotnetStyleRequireAccessibilityModifiersStyle =
					this.Set.DotnetStyleRequireAccessibilityModifiersStyle.Update(
						new NodeInformation<MemberDeclarationSyntax>(node));
				base.VisitStructDeclaration(node);
			}

			public override void VisitToken(SyntaxToken token)
			{
				this.Set.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle =
					this.Set.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle.Update(
						new TokenInformation(token));
				base.VisitToken(token);
			}

			public override void VisitTupleExpression(TupleExpressionSyntax node)
			{
				this.Set.DotnetStylePreferInferredTupleNamesStyle =
					this.Set.DotnetStylePreferInferredTupleNamesStyle.Update(
						new NodeInformation<TupleExpressionSyntax>(node));
				base.VisitTupleExpression(node);
			}

			public StyleSet Set { get; } = new StyleSet();
		}

		private sealed class StyleSet
			: IStyleSet
		{
			public IStyleSet Update(IStyleSet set) =>
				new StyleSet()
				{
					CSharpNewLineBeforeCatchStyle = 
						this.CSharpNewLineBeforeCatchStyle.Add(set.CSharpNewLineBeforeCatchStyle),
					CSharpNewLineBeforeElseStyle = 
						this.CSharpNewLineBeforeElseStyle.Add(set.CSharpNewLineBeforeElseStyle),
					CSharpNewLineBeforeFinallyStyle =
						this.CSharpNewLineBeforeFinallyStyle.Add(set.CSharpNewLineBeforeFinallyStyle),
					CSharpNewLineBeforeMembersInAnonymousTypesStyle =
						this.CSharpNewLineBeforeMembersInAnonymousTypesStyle.Add(set.CSharpNewLineBeforeMembersInAnonymousTypesStyle),
					CSharpNewLineBeforeMembersInObjectInitializersStyle =
						this.CSharpNewLineBeforeMembersInObjectInitializersStyle.Add(set.CSharpNewLineBeforeMembersInObjectInitializersStyle),
					CSharpNewLineBetweenQueryExpressionClausesStyle =
						this.CSharpNewLineBetweenQueryExpressionClausesStyle.Add(set.CSharpNewLineBetweenQueryExpressionClausesStyle),
					CSharpPreferBracesStyle =
						this.CSharpPreferBracesStyle.Add(set.CSharpPreferBracesStyle),
					CSharpPreferredModifierOrderStyle =
						this.CSharpPreferredModifierOrderStyle.Add(set.CSharpPreferredModifierOrderStyle),
					CSharpPreferSimpleDefaultExpressionStyle =
						this.CSharpPreferSimpleDefaultExpressionStyle.Add(set.CSharpPreferSimpleDefaultExpressionStyle),
					CSharpPreserveSingleLineBlocksStyle =
						this.CSharpPreserveSingleLineBlocksStyle.Add(set.CSharpPreserveSingleLineBlocksStyle),
					CSharpSpaceAfterCastStyle =
						this.CSharpSpaceAfterCastStyle.Add(set.CSharpSpaceAfterCastStyle),
					CSharpSpaceAfterKeywordsInControlFlowStatementsStyle =
						this.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle.Add(set.CSharpSpaceAfterKeywordsInControlFlowStatementsStyle),
					CSharpSpaceBetweenMethodCallParameterListParenthesesStyle =
						this.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle.Add(set.CSharpSpaceBetweenMethodCallParameterListParenthesesStyle),
					CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle =
						this.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle.Add(set.CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle),
					CSharpSpaceBetweenParenthesesStyle =
						this.CSharpSpaceBetweenParenthesesStyle.Add(set.CSharpSpaceBetweenParenthesesStyle),
					CSharpStyleExpressionBodiedAccessorsStyle =
						this.CSharpStyleExpressionBodiedAccessorsStyle.Add(set.CSharpStyleExpressionBodiedAccessorsStyle),
					CSharpStyleExpressionBodiedConstructorsStyle =
						this.CSharpStyleExpressionBodiedConstructorsStyle.Add(set.CSharpStyleExpressionBodiedConstructorsStyle),
					CSharpStyleExpressionBodiedIndexersStyle =
						this.CSharpStyleExpressionBodiedIndexersStyle.Add(set.CSharpStyleExpressionBodiedIndexersStyle),
					CSharpStyleExpressionBodiedMethodsStyle =
						this.CSharpStyleExpressionBodiedMethodsStyle.Add(set.CSharpStyleExpressionBodiedMethodsStyle),
					CSharpStyleExpressionBodiedOperatorsStyle =
						this.CSharpStyleExpressionBodiedOperatorsStyle.Add(set.CSharpStyleExpressionBodiedOperatorsStyle),
					CSharpStyleExpressionBodiedPropertiesStyle =
						this.CSharpStyleExpressionBodiedPropertiesStyle.Add(set.CSharpStyleExpressionBodiedPropertiesStyle),
					CSharpStyleInlinedVariableDeclarationStyle =
						this.CSharpStyleInlinedVariableDeclarationStyle.Add(set.CSharpStyleInlinedVariableDeclarationStyle),
					CSharpStylePatternLocalOverAnonymousFunctionStyle =
						this.CSharpStylePatternLocalOverAnonymousFunctionStyle.Add(set.CSharpStylePatternLocalOverAnonymousFunctionStyle),
					CSharpStyleVarForBuiltInTypesStyle =
						this.CSharpStyleVarForBuiltInTypesStyle.Add(set.CSharpStyleVarForBuiltInTypesStyle),
					CSharpStyleVarWhenTypeIsApparentStyle =
						this.CSharpStyleVarWhenTypeIsApparentStyle.Add(set.CSharpStyleVarWhenTypeIsApparentStyle),
					DotnetSortSystemDirectivesFirstStyle =
						this.DotnetSortSystemDirectivesFirstStyle.Add(set.DotnetSortSystemDirectivesFirstStyle),
					DotnetStyleExplicitTupleNamesStyle =
						this.DotnetStyleExplicitTupleNamesStyle.Add(set.DotnetStyleExplicitTupleNamesStyle),
					DotnetStyleObjectInitializerStyle =
						this.DotnetStyleObjectInitializerStyle.Add(set.DotnetStyleObjectInitializerStyle),
					DotnetStylePredefinedTypeForLocalsParametersMembersStyle =
						this.DotnetStylePredefinedTypeForLocalsParametersMembersStyle.Add(set.DotnetStylePredefinedTypeForLocalsParametersMembersStyle),
					DotnetStylePredefinedTypeForMemberAccessStyle =
						this.DotnetStylePredefinedTypeForMemberAccessStyle.Add(set.DotnetStylePredefinedTypeForMemberAccessStyle),
					DotnetStylePreferInferredAnonymousTypeMemberNamesStyle =
						this.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle.Add(set.DotnetStylePreferInferredAnonymousTypeMemberNamesStyle),
					DotnetStylePreferInferredTupleNamesStyle = 
						this.DotnetStylePreferInferredTupleNamesStyle.Add(set.DotnetStylePreferInferredTupleNamesStyle),
					DotnetStyleQualificationForEventStyle =
						this.DotnetStyleQualificationForEventStyle.Add(set.DotnetStyleQualificationForEventStyle),
					DotnetStyleQualificationForFieldStyle =
						this.DotnetStyleQualificationForFieldStyle.Add(set.DotnetStyleQualificationForFieldStyle),
					DotnetStyleQualificationForMethodStyle =
						this.DotnetStyleQualificationForMethodStyle.Add(set.DotnetStyleQualificationForMethodStyle),
					DotnetStyleQualificationForPropertyStyle =
						this.DotnetStyleQualificationForPropertyStyle.Add(set.DotnetStyleQualificationForPropertyStyle),
					DotnetStyleRequireAccessibilityModifiersStyle = 
						this.DotnetStyleRequireAccessibilityModifiersStyle.Add(set.DotnetStyleRequireAccessibilityModifiersStyle),
					IndentStyleStyle =
						this.IndentStyleStyle.Add(set.IndentStyleStyle)
				};

			public CSharpNewLineBeforeCatchStyle CSharpNewLineBeforeCatchStyle { get; set; } =
				new CSharpNewLineBeforeCatchStyle(new BooleanData());
			public CSharpNewLineBeforeElseStyle CSharpNewLineBeforeElseStyle { get; set; } =
				new CSharpNewLineBeforeElseStyle(new BooleanData());
			public CSharpNewLineBeforeFinallyStyle CSharpNewLineBeforeFinallyStyle { get; set; } =
				new CSharpNewLineBeforeFinallyStyle(new BooleanData());
			public CSharpNewLineBeforeMembersInAnonymousTypesStyle CSharpNewLineBeforeMembersInAnonymousTypesStyle { get; set; } =
				new CSharpNewLineBeforeMembersInAnonymousTypesStyle(new BooleanData());
			public CSharpNewLineBeforeMembersInObjectInitializersStyle CSharpNewLineBeforeMembersInObjectInitializersStyle { get; set; } =
				new CSharpNewLineBeforeMembersInObjectInitializersStyle(new BooleanData());
			public CSharpNewLineBetweenQueryExpressionClausesStyle CSharpNewLineBetweenQueryExpressionClausesStyle { get; set; } =
				new CSharpNewLineBetweenQueryExpressionClausesStyle(new BooleanData());
			public CSharpPreferBracesStyle CSharpPreferBracesStyle { get; set; } =
				new CSharpPreferBracesStyle(new BooleanData());
			public CSharpPreferredModifierOrderStyle CSharpPreferredModifierOrderStyle { get; set; } =
				new CSharpPreferredModifierOrderStyle(new ModifierData());
			public CSharpPreferSimpleDefaultExpressionStyle CSharpPreferSimpleDefaultExpressionStyle { get; set; } =
				new CSharpPreferSimpleDefaultExpressionStyle(new BooleanData());
			public CSharpPreserveSingleLineBlocksStyle CSharpPreserveSingleLineBlocksStyle { get; set; } =
				new CSharpPreserveSingleLineBlocksStyle(new BooleanData());
			public CSharpSpaceAfterCastStyle CSharpSpaceAfterCastStyle { get; set; } =
				new CSharpSpaceAfterCastStyle(new BooleanData());
			public CSharpSpaceAfterKeywordsInControlFlowStatementsStyle CSharpSpaceAfterKeywordsInControlFlowStatementsStyle { get; set; } =
				new CSharpSpaceAfterKeywordsInControlFlowStatementsStyle(new BooleanData());
			public CSharpSpaceBetweenMethodCallParameterListParenthesesStyle CSharpSpaceBetweenMethodCallParameterListParenthesesStyle { get; set; } =
				new CSharpSpaceBetweenMethodCallParameterListParenthesesStyle(new BooleanData());
			public CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle { get; set; } =
				new CSharpSpaceBetweenMethodDeclarationParameterListParenthesesStyle(new BooleanData());
			public CSharpSpaceBetweenParenthesesStyle CSharpSpaceBetweenParenthesesStyle { get; set; } =
				new CSharpSpaceBetweenParenthesesStyle(new ParenthesesSpaceData());
			public CSharpStyleExpressionBodiedAccessorsStyle CSharpStyleExpressionBodiedAccessorsStyle { get; set; } =
				new CSharpStyleExpressionBodiedAccessorsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedConstructorsStyle CSharpStyleExpressionBodiedConstructorsStyle { get; set; } =
				new CSharpStyleExpressionBodiedConstructorsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedIndexersStyle CSharpStyleExpressionBodiedIndexersStyle { get; set; } =
				new CSharpStyleExpressionBodiedIndexersStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedMethodsStyle CSharpStyleExpressionBodiedMethodsStyle { get; set; } =
				new CSharpStyleExpressionBodiedMethodsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedOperatorsStyle CSharpStyleExpressionBodiedOperatorsStyle { get; set; } =
				new CSharpStyleExpressionBodiedOperatorsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedPropertiesStyle CSharpStyleExpressionBodiedPropertiesStyle { get; set; } =
				new CSharpStyleExpressionBodiedPropertiesStyle(new ExpressionBodiedData());
			public CSharpStyleInlinedVariableDeclarationStyle CSharpStyleInlinedVariableDeclarationStyle { get; set; } =
				new CSharpStyleInlinedVariableDeclarationStyle(new BooleanData());
			public CSharpStylePatternLocalOverAnonymousFunctionStyle CSharpStylePatternLocalOverAnonymousFunctionStyle { get; set; } =
				new CSharpStylePatternLocalOverAnonymousFunctionStyle(new BooleanData());
			public CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; set; } =
				new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
			public CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; set; } =
				new CSharpStyleVarWhenTypeIsApparentStyle(new BooleanData());
			public DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; set; } =
				new DotnetSortSystemDirectivesFirstStyle(new BooleanData());
			public DotnetStyleExplicitTupleNamesStyle DotnetStyleExplicitTupleNamesStyle { get; set; } =
				new DotnetStyleExplicitTupleNamesStyle(new BooleanData());
			public DotnetStyleObjectInitializerStyle DotnetStyleObjectInitializerStyle { get; set; } =
				new DotnetStyleObjectInitializerStyle(new BooleanData());
			public DotnetStylePredefinedTypeForLocalsParametersMembersStyle DotnetStylePredefinedTypeForLocalsParametersMembersStyle { get; set; } =
				new DotnetStylePredefinedTypeForLocalsParametersMembersStyle(new BooleanData());
			public DotnetStylePredefinedTypeForMemberAccessStyle DotnetStylePredefinedTypeForMemberAccessStyle { get; set; } =
				new DotnetStylePredefinedTypeForMemberAccessStyle(new BooleanData());
			public DotnetStylePreferInferredAnonymousTypeMemberNamesStyle DotnetStylePreferInferredAnonymousTypeMemberNamesStyle { get; set; } =
				new DotnetStylePreferInferredAnonymousTypeMemberNamesStyle(new BooleanData());
			public DotnetStylePreferInferredTupleNamesStyle DotnetStylePreferInferredTupleNamesStyle { get; set; } =
				new DotnetStylePreferInferredTupleNamesStyle(new BooleanData());
			public DotnetStyleQualificationForEventStyle DotnetStyleQualificationForEventStyle { get; set; } =
				new DotnetStyleQualificationForEventStyle(new BooleanData());
			public DotnetStyleQualificationForFieldStyle DotnetStyleQualificationForFieldStyle { get; set; } =
				new DotnetStyleQualificationForFieldStyle(new BooleanData());
			public DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; set; } =
				new DotnetStyleQualificationForMethodStyle(new BooleanData());
			public DotnetStyleQualificationForPropertyStyle DotnetStyleQualificationForPropertyStyle { get; set; } =
				new DotnetStyleQualificationForPropertyStyle(new BooleanData());
			public DotnetStyleRequireAccessibilityModifiersStyle DotnetStyleRequireAccessibilityModifiersStyle { get; set; } =
				new DotnetStyleRequireAccessibilityModifiersStyle(new BooleanData());
			public IndentStyleStyle IndentStyleStyle { get; set; } =
				new IndentStyleStyle(new TabSpaceData());
		}
	}
}
