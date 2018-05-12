using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.SyntaxNodeExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStyleObjectInitializerStyle
		: ModelSeverityNodeStyle<BooleanData, ObjectCreationExpressionSyntax, ModelNodeInformation<ObjectCreationExpressionSyntax>, DotnetStyleObjectInitializerStyle>
	{
		public DotnetStyleObjectInitializerStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleObjectInitializerStyle Add(DotnetStyleObjectInitializerStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleObjectInitializerStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_style_object_initializer = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleObjectInitializerStyle Update(ModelNodeInformation<ObjectCreationExpressionSyntax> information)
		{
			var (node, model) = information ?? throw new ArgumentNullException(nameof(information));

			if (!node.ContainsDiagnostics)
			{
				if(node.ChildNodes().Any(_ => _.IsKind(SyntaxKind.ObjectInitializerExpression)))
				{
					return new DotnetStyleObjectInitializerStyle(this.Data.Update(true), this.Severity);
				}
				else
				{
					// "This where the syntax visualizer mentioned by Jason Malinkwski becomes useful. Using it, 
					// you can see that the first case is a LocalDeclarationStatement containing VariableDeclaration containing one VariableDeclarator, 
					// while the second case is ExpressionStatement containing SimpleAssignmentExpression. – svick Mar 14 '17 at 17:59 "

					// First case: is the object being assigned to a local variable (VariableDeclarator), 
					// or a field/property (SimpleAssignmentExpression ->?
					// Then, is the next node an ExpressionStatement that contains a SimpleAssignmentExpression
					var assignment = node.FindParent<VariableDeclaratorSyntax>() ??
						node.FindParent<AssignmentExpressionSyntax>()?.ChildNodes()
							.FirstOrDefault(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression));

					if (assignment != null)
					{
						var assignmentSymbol = model.GetDeclaredSymbol(assignment);

						if(assignmentSymbol != null)
						{
							var statement = assignment.FindParent<StatementSyntax>();

							if (statement != null)
							{
								var parentStatement = statement.Parent;
								var siblings = parentStatement.ChildNodes().ToArray();
								var statementIndex = Array.IndexOf(siblings, node);
								var nextNode = siblings[statementIndex + 1];

								if (nextNode is ExpressionStatementSyntax &&
									nextNode.ChildNodes().Any(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression)))
								{
									// Find the first IdentifierName, and get its' symbol and see if it matches.
									var name = nextNode.DescendantNodes().FirstOrDefault(_ => _.IsKind(SyntaxKind.IdentifierName));

									if (name != null)
									{
										return new DotnetStyleObjectInitializerStyle(
											this.Data.Update(model.GetSymbolInfo(name).Symbol == assignmentSymbol), this.Severity);
									}
								}
							}
						}
					}
				}

				return new DotnetStyleObjectInitializerStyle(this.Data, this.Severity);
			}

			return new DotnetStyleObjectInitializerStyle(this.Data, this.Severity);
		}

		public class Customer
		{
			private Customer child = new Customer();
			public int Age { get; set; }

			public void Foo()
			{
				// Note: IDE0017 is the "thing" that kicks in.
				// https://github.com/dotnet/roslyn/blob/master/src/Features/Core/Portable/UseObjectInitializer/AbstractUseObjectInitializerDiagnosticAnalyzer.cs
				// http://source.roslyn.io/#Microsoft.CodeAnalysis.Features/UseObjectInitializer/AbstractUseObjectInitializerDiagnosticAnalyzer.cs,b300eec6b11db5e1

				// dotnet_style_object_initializer = true
				var c = new Customer() { Age = 21 };

				// dotnet_style_object_initializer = false
				// https://stackoverflow.com/questions/42766977/why-cant-i-get-all-objectcreationexpressionsyntax-if-initialized-as-null
				// "This where the syntax visualizer mentioned by Jason Malinkwski becomes useful. Using it, 
				// you can see that the first case is a LocalDeclarationStatement containing VariableDeclaration containing one VariableDeclarator, 
				// while the second case is ExpressionStatement containing SimpleAssignmentExpression. – svick Mar 14 '17 at 17:59 "
				var c2 = new Customer();
				c2.Age = 10;

				this.child = new Customer();
				this.child.Age = 21;
				this.child.Foo();
				this.Foo();
			}
		}
	}
}
