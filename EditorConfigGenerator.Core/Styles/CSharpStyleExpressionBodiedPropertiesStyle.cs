using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedPropertiesStyle
		: SeverityNodeStyle<ExpressionBodiedData, PropertyDeclarationSyntax, NodeInformation<PropertyDeclarationSyntax>, CSharpStyleExpressionBodiedPropertiesStyle>
	{
		public const string Setting = "csharp_style_expression_bodied_properties";

		public CSharpStyleExpressionBodiedPropertiesStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedPropertiesStyle Add(CSharpStyleExpressionBodiedPropertiesStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedPropertiesStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting(CSharpStyleExpressionBodiedPropertiesStyle.Setting, this.Severity);

		public override CSharpStyleExpressionBodiedPropertiesStyle Update(NodeInformation<PropertyDeclarationSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			if (information.Node.DescendantNodes()
				.SingleOrDefault(_ => _.Kind() == SyntaxKind.GetAccessorDeclaration) is AccessorDeclarationSyntax getAccessor)
			{
				var accessorStyle = new CSharpStyleExpressionBodiedAccessorsStyle(this.Data, this.Severity);

				return new CSharpStyleExpressionBodiedPropertiesStyle(accessorStyle.Update(
					new NodeInformation<AccessorDeclarationSyntax>(getAccessor)).Data, this.Severity);
			}
			else
			{
				return new CSharpStyleExpressionBodiedPropertiesStyle(information.Node.Examine(this.Data), this.Severity);
			}
		}
	}
}