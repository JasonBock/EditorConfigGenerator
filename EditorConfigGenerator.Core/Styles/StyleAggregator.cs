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
			AppendSetting(this.Set.CSharpStyleExpressionBodiedAccessorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedConstructorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedIndexersStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedMethodsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedOperatorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedPropertiesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleInlinedVariableDeclarationStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarForBuiltInTypesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarWhenTypeIsApparentStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetSortSystemDirectivesFirstStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleExplicitTupleNamesStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForEventStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForFieldStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForMethodStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetStyleQualificationForPropertyStyle.GetSetting(), builder);
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
				this.Set.IndentStyleStyle = 
					this.Set.IndentStyleStyle.Update(node);
				this.Set.DotnetStyleQualificationForEventStyle =
					this.Set.DotnetStyleQualificationForEventStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStyleQualificationForFieldStyle =
					this.Set.DotnetStyleQualificationForFieldStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				this.Set.DotnetStyleQualificationForPropertyStyle =
					this.Set.DotnetStyleQualificationForPropertyStyle.Update(
						new ModelNodeInformation<SyntaxNode>(node, this.model));
				base.Visit(node);
			}

			public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedAccessorsStyle =
					this.Set.CSharpStyleExpressionBodiedAccessorsStyle.Update(node);
				base.VisitAccessorDeclaration(node);
			}

			public override void VisitArgument(ArgumentSyntax node)
			{
				this.Set.CSharpStyleInlinedVariableDeclarationStyle =
					this.Set.CSharpStyleInlinedVariableDeclarationStyle.Update(node);
				base.VisitArgument(node);
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
				base.VisitConstructorDeclaration(node);
			}

			public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedIndexersStyle =
					this.Set.CSharpStyleExpressionBodiedIndexersStyle.Update(node);
				base.VisitIndexerDeclaration(node);
			}

			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				this.Set.DotnetStyleQualificationForMethodStyle =
					this.Set.DotnetStyleQualificationForMethodStyle.Update(
						new ModelNodeInformation<InvocationExpressionSyntax>(node, this.model));
				base.VisitInvocationExpression(node);
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
				base.VisitMemberAccessExpression(node);
			}

			public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedMethodsStyle =
					this.Set.CSharpStyleExpressionBodiedMethodsStyle.Update(node);
				base.VisitMethodDeclaration(node);
			}

			public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedOperatorsStyle =
					this.Set.CSharpStyleExpressionBodiedOperatorsStyle.Update(node);
				base.VisitOperatorDeclaration(node);
			}

			public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
			{
				this.Set.CSharpStyleExpressionBodiedPropertiesStyle =
					this.Set.CSharpStyleExpressionBodiedPropertiesStyle.Update(node);
				base.VisitPropertyDeclaration(node);
			}

			public StyleSet Set { get; } = new StyleSet();
		}

		private sealed class StyleSet
			: IStyleSet
		{
			public IStyleSet Update(IStyleSet set) =>
				new StyleSet()
				{
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
					CSharpStyleVarForBuiltInTypesStyle =
						this.CSharpStyleVarForBuiltInTypesStyle.Add(set.CSharpStyleVarForBuiltInTypesStyle),
					CSharpStyleVarWhenTypeIsApparentStyle =
						this.CSharpStyleVarWhenTypeIsApparentStyle.Add(set.CSharpStyleVarWhenTypeIsApparentStyle),
					DotnetSortSystemDirectivesFirstStyle =
						this.DotnetSortSystemDirectivesFirstStyle.Add(set.DotnetSortSystemDirectivesFirstStyle),
					DotnetStyleExplicitTupleNamesStyle =
						this.DotnetStyleExplicitTupleNamesStyle.Add(set.DotnetStyleExplicitTupleNamesStyle),
					DotnetStyleQualificationForEventStyle =
						this.DotnetStyleQualificationForEventStyle.Add(set.DotnetStyleQualificationForEventStyle),
					DotnetStyleQualificationForFieldStyle =
						this.DotnetStyleQualificationForFieldStyle.Add(set.DotnetStyleQualificationForFieldStyle),
					DotnetStyleQualificationForMethodStyle =
						this.DotnetStyleQualificationForMethodStyle.Add(set.DotnetStyleQualificationForMethodStyle),
					DotnetStyleQualificationForPropertyStyle =
						this.DotnetStyleQualificationForPropertyStyle.Add(set.DotnetStyleQualificationForPropertyStyle),
					IndentStyleStyle =
						this.IndentStyleStyle.Add(set.IndentStyleStyle)
				};

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
			public CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; set; } =
				new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
			public CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; set; } =
				new CSharpStyleVarWhenTypeIsApparentStyle(new BooleanData());
			public DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; set; } =
				new DotnetSortSystemDirectivesFirstStyle(new BooleanData());
			public DotnetStyleExplicitTupleNamesStyle DotnetStyleExplicitTupleNamesStyle { get; set; } =
				new DotnetStyleExplicitTupleNamesStyle(new BooleanData());
			public DotnetStyleQualificationForEventStyle DotnetStyleQualificationForEventStyle { get; set; } =
				new DotnetStyleQualificationForEventStyle(new BooleanData());
			public DotnetStyleQualificationForFieldStyle DotnetStyleQualificationForFieldStyle { get; set; } =
				new DotnetStyleQualificationForFieldStyle(new BooleanData());
			public DotnetStyleQualificationForMethodStyle DotnetStyleQualificationForMethodStyle { get; set; } =
				new DotnetStyleQualificationForMethodStyle(new BooleanData());
			public DotnetStyleQualificationForPropertyStyle DotnetStyleQualificationForPropertyStyle { get; set; } =
				new DotnetStyleQualificationForPropertyStyle(new BooleanData());
			public IndentStyleStyle IndentStyleStyle { get; set; } =
				new IndentStyleStyle(new TabSpaceData());
		}
	}
}
