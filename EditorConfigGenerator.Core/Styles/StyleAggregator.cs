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
			AppendSetting(this.Set.CSharpStyleExpressionBodiedConstructorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedMethodsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleExpressionBodiedOperatorsStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleInlinedVariableDeclarationStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarForBuiltInTypesStyle.GetSetting(), builder);
			AppendSetting(this.Set.CSharpStyleVarWhenTypeIsApparentStyle.GetSetting(), builder);
			AppendSetting(this.Set.DotnetSortSystemDirectivesFirstStyle.GetSetting(), builder);
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
				base.Visit(node);
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

			public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
			{
				this.Set.CSharpStyleVarForBuiltInTypesStyle =
					this.Set.CSharpStyleVarForBuiltInTypesStyle.Update(node);
				this.Set.CSharpStyleVarWhenTypeIsApparentStyle =
					this.Set.CSharpStyleVarWhenTypeIsApparentStyle.Update(node);
				base.VisitLocalDeclarationStatement(node);
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

			public StyleSet Set { get; } = new StyleSet();
		}

		private sealed class StyleSet 
			: IStyleSet
		{
			public IStyleSet Update(IStyleSet set) =>
				new StyleSet()
				{
					CSharpStyleExpressionBodiedConstructorsStyle =
						this.CSharpStyleExpressionBodiedConstructorsStyle.Add(set.CSharpStyleExpressionBodiedConstructorsStyle),
					CSharpStyleExpressionBodiedMethodsStyle =
						this.CSharpStyleExpressionBodiedMethodsStyle.Add(set.CSharpStyleExpressionBodiedMethodsStyle),
					CSharpStyleExpressionBodiedOperatorsStyle =
						this.CSharpStyleExpressionBodiedOperatorsStyle.Add(set.CSharpStyleExpressionBodiedOperatorsStyle),
					CSharpStyleInlinedVariableDeclarationStyle =
						this.CSharpStyleInlinedVariableDeclarationStyle.Add(set.CSharpStyleInlinedVariableDeclarationStyle),
					CSharpStyleVarForBuiltInTypesStyle =
						this.CSharpStyleVarForBuiltInTypesStyle.Add(set.CSharpStyleVarForBuiltInTypesStyle),
					CSharpStyleVarWhenTypeIsApparentStyle =
						this.CSharpStyleVarWhenTypeIsApparentStyle.Add(set.CSharpStyleVarWhenTypeIsApparentStyle),
					DotnetSortSystemDirectivesFirstStyle =
						this.DotnetSortSystemDirectivesFirstStyle.Add(set.DotnetSortSystemDirectivesFirstStyle),
					IndentStyleStyle =
						this.IndentStyleStyle.Add(set.IndentStyleStyle)
				};

			public CSharpStyleExpressionBodiedConstructorsStyle CSharpStyleExpressionBodiedConstructorsStyle { get; set; } =
				new CSharpStyleExpressionBodiedConstructorsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedMethodsStyle CSharpStyleExpressionBodiedMethodsStyle { get; set; } =
				new CSharpStyleExpressionBodiedMethodsStyle(new ExpressionBodiedData());
			public CSharpStyleExpressionBodiedOperatorsStyle CSharpStyleExpressionBodiedOperatorsStyle { get; set; } =
				new CSharpStyleExpressionBodiedOperatorsStyle(new ExpressionBodiedData());
			public CSharpStyleInlinedVariableDeclarationStyle CSharpStyleInlinedVariableDeclarationStyle { get; set; } =
				new CSharpStyleInlinedVariableDeclarationStyle(new BooleanData());
			public CSharpStyleVarForBuiltInTypesStyle CSharpStyleVarForBuiltInTypesStyle { get; set; } =
				new CSharpStyleVarForBuiltInTypesStyle(new BooleanData());
			public CSharpStyleVarWhenTypeIsApparentStyle CSharpStyleVarWhenTypeIsApparentStyle { get; set; } =
				new CSharpStyleVarWhenTypeIsApparentStyle(new BooleanData());
			public DotnetSortSystemDirectivesFirstStyle DotnetSortSystemDirectivesFirstStyle { get; set; } =
				new DotnetSortSystemDirectivesFirstStyle(new BooleanData());
			public IndentStyleStyle IndentStyleStyle { get; set; } =
				new IndentStyleStyle(new TabSpaceData());
		}
	}
}
