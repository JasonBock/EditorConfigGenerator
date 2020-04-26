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
		public const string Setting = "dotnet_style_object_initializer";

		public DotnetStyleObjectInitializerStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleObjectInitializerStyle Add(DotnetStyleObjectInitializerStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleObjectInitializerStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{DotnetStyleObjectInitializerStyle.Setting} = {value}:{this.Severity.GetDescription()}";
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
				if (node.ChildNodes().Any(_ => _.IsKind(SyntaxKind.ObjectInitializerExpression)))
				{
					return new DotnetStyleObjectInitializerStyle(this.Data.Update(true), this.Severity);
				}
				else
				{
					var assignment = node.FindParent<VariableDeclaratorSyntax>() ??
						node.FindParent<AssignmentExpressionSyntax>()?.ChildNodes()
							.FirstOrDefault(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression));

					if (assignment is { })
					{
						var assignmentSymbol = model.GetDeclaredSymbol(assignment);

						if (assignmentSymbol is { })
						{
							var statement = assignment.FindParent<StatementSyntax>();

							if (statement is { })
							{
								var parentStatement = statement.Parent;

								if (parentStatement is { })
								{
									var siblings = parentStatement.ChildNodes().ToArray();

									if (siblings.Length > 1)
									{
										var statementIndex = Array.IndexOf(siblings, statement);

										if (statementIndex < siblings.Length - 1)
										{
											var nextNode = siblings[statementIndex + 1];

											if (nextNode is ExpressionStatementSyntax &&
												nextNode.ChildNodes().Any(_ => _.IsKind(SyntaxKind.SimpleAssignmentExpression)))
											{
												var name = nextNode.DescendantNodes().FirstOrDefault(_ => _.IsKind(SyntaxKind.IdentifierName));

												if (name is { })
												{
													var isSameSymbol = object.ReferenceEquals(model.GetSymbolInfo(name).Symbol, assignmentSymbol);

													if (isSameSymbol)
													{
														return new DotnetStyleObjectInitializerStyle(this.Data.Update(false), this.Severity);
													}
												}
											}
										}
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
	}
}
