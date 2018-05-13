using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleVarWhenTypeIsApparentStyle
		: SeverityNodeStyle<BooleanData, LocalDeclarationStatementSyntax, NodeInformation<LocalDeclarationStatementSyntax>, CSharpStyleVarWhenTypeIsApparentStyle>
	{
		public const string Setting = "csharp_style_var_when_type_is_apparent";

		public CSharpStyleVarWhenTypeIsApparentStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleVarWhenTypeIsApparentStyle Add(CSharpStyleVarWhenTypeIsApparentStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleVarWhenTypeIsApparentStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"{CSharpStyleVarWhenTypeIsApparentStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpStyleVarWhenTypeIsApparentStyle Update(NodeInformation<LocalDeclarationStatementSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				if (node.DescendantNodes()
					.Any(_ => _.Kind() == SyntaxKind.ObjectCreationExpression))
				{
					var variableDeclaration = node.ChildNodes()
						.Single(_ => _.Kind() == SyntaxKind.VariableDeclaration);
					var identifierName = variableDeclaration.ChildNodes()
						.SingleOrDefault(_ => _.Kind() == SyntaxKind.IdentifierName);

					return identifierName != null ?
						new CSharpStyleVarWhenTypeIsApparentStyle(this.Data.Update((identifierName as IdentifierNameSyntax).IsVar), this.Severity) :
						new CSharpStyleVarWhenTypeIsApparentStyle(this.Data.Update(false), this.Severity);
				}
			}

			return new CSharpStyleVarWhenTypeIsApparentStyle(this.Data, this.Severity);
		}
	}
}