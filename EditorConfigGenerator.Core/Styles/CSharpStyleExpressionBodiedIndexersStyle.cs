using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedIndexersStyle
		: SeverityNodeStyle<ExpressionBodiedData, IndexerDeclarationSyntax, NodeInformation<IndexerDeclarationSyntax>, CSharpStyleExpressionBodiedIndexersStyle>
	{
		public const string Setting = "csharp_style_expression_bodied_indexers";

		public CSharpStyleExpressionBodiedIndexersStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedIndexersStyle Add(CSharpStyleExpressionBodiedIndexersStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedIndexersStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting(CSharpStyleExpressionBodiedIndexersStyle.Setting, this.Severity);

		public override CSharpStyleExpressionBodiedIndexersStyle Update(NodeInformation<IndexerDeclarationSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			if (information.Node.DescendantNodes()
				.SingleOrDefault(_ => _.Kind() == SyntaxKind.GetAccessorDeclaration) is AccessorDeclarationSyntax getAccessor)
			{
				var accessorStyle = new CSharpStyleExpressionBodiedAccessorsStyle(this.Data, this.Severity);

				return new CSharpStyleExpressionBodiedIndexersStyle(accessorStyle.Update(
					new NodeInformation<AccessorDeclarationSyntax>(getAccessor)).Data, this.Severity);
			}
			else
			{
				return new CSharpStyleExpressionBodiedIndexersStyle(information.Node.Examine(this.Data), this.Severity);
			}
		}
	}
}