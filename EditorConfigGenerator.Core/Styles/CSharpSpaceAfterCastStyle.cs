using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpSpaceAfterCastStyle
		: SeverityStyle<BooleanData, CastExpressionSyntax, NodeInformation<CastExpressionSyntax>, CSharpSpaceAfterCastStyle>
	{
		public CSharpSpaceAfterCastStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override CSharpSpaceAfterCastStyle Add(CSharpSpaceAfterCastStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpSpaceAfterCastStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"csharp_space_after_cast = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpSpaceAfterCastStyle Update(NodeInformation<CastExpressionSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var closeParen = node.ChildTokens().Single(_ => _.RawKind == (int)SyntaxKind.CloseParenToken);
				return new CSharpSpaceAfterCastStyle(this.Data.Update(closeParen.HasTrailingTrivia), this.Severity);
			}

			return new CSharpSpaceAfterCastStyle(this.Data, this.Severity);
		}
	}
}
