using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStyleQualificationForMethodStyle
		: ModelSeverityStyle<BooleanData, InvocationExpressionSyntax, DotnetStyleQualificationForMethodStyle>
	{
		public DotnetStyleQualificationForMethodStyle(BooleanData data, SemanticModel model, Severity severity = Severity.Error)
			: base(data, model, severity) { }

		public override DotnetStyleQualificationForMethodStyle Add(DotnetStyleQualificationForMethodStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleQualificationForMethodStyle(this.Data.Add(style.Data), this.Model, this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_style_qualification_for_method = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleQualificationForMethodStyle Update(InvocationExpressionSyntax node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			if (!node.ContainsDiagnostics)
			{
				var simpleMember = node.DescendantNodes().FirstOrDefault(_ => _.Kind() == SyntaxKind.SimpleMemberAccessExpression);

				if (simpleMember != null)
				{
					// This could be a static call with the class name or a "this" call to either
					// an instance method or an extension method.
					if (simpleMember.DescendantNodes().Any(_ => _.Kind() == SyntaxKind.ThisExpression))
					{
						// Check to see if it's an extension method.
						// TODO: This would be "easier" (and maybe the only way) if we had a semantic model, but...
						// the point is that this only gets a syntax node. And even if I did a compilation creation
						// to get that model, even with a .cs file, I don't know if I'd get that automatically.
						return new DotnetStyleQualificationForMethodStyle(this.Data.Update(true), this.Model, this.Severity);
					}
					else
					{
						return new DotnetStyleQualificationForMethodStyle(this.Data, this.Model, this.Severity);
					}
				}
				else
				{
					// This could be a call to either a static or instance method. If it's instance, then 
					// it has to be a "this" call without the "this".
					// As above, we need a sematic model to determine if this is a "false" or a static call,
					// which we wouldn't care about.
					return new DotnetStyleQualificationForMethodStyle(this.Data.Update(false), this.Model, this.Severity);
				}
			}
			else
			{
				return new DotnetStyleQualificationForMethodStyle(this.Data, this.Model, this.Severity);
			}
		}
	}
}