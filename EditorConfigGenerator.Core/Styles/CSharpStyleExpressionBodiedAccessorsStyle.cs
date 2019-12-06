using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpStyleExpressionBodiedAccessorsStyle
		: SeverityNodeStyle<ExpressionBodiedData, AccessorDeclarationSyntax, NodeInformation<AccessorDeclarationSyntax>, CSharpStyleExpressionBodiedAccessorsStyle>
	{
		public const string Setting = "csharp_style_expression_bodied_accessors";

		public CSharpStyleExpressionBodiedAccessorsStyle(ExpressionBodiedData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpStyleExpressionBodiedAccessorsStyle Add(CSharpStyleExpressionBodiedAccessorsStyle style)
		{
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpStyleExpressionBodiedAccessorsStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting(CSharpStyleExpressionBodiedAccessorsStyle.Setting, this.Severity);

		public override CSharpStyleExpressionBodiedAccessorsStyle Update(NodeInformation<AccessorDeclarationSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }
			return new CSharpStyleExpressionBodiedAccessorsStyle(information.Node.Examine(this.Data), this.Severity);
		}
	}
}