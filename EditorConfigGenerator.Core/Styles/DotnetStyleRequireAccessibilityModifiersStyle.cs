using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStyleRequireAccessibilityModifiersStyle
		: SeverityNodeStyle<BooleanData, MemberDeclarationSyntax, NodeInformation<MemberDeclarationSyntax>, DotnetStyleRequireAccessibilityModifiersStyle>
	{
		public const string Setting = "dotnet_style_require_accessibility_modifiers";
		private static readonly string[] AccessibilityModifiers = new[] { "public", "protected", "internal", "private" };

		public DotnetStyleRequireAccessibilityModifiersStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleRequireAccessibilityModifiersStyle Add(DotnetStyleRequireAccessibilityModifiersStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "always" : "never";
				return $"{DotnetStyleRequireAccessibilityModifiersStyle.Setting} = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStyleRequireAccessibilityModifiersStyle Update(NodeInformation<MemberDeclarationSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var hasAccessibility = node.ChildTokens().Any(
					_ => DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifiers.Contains(_.Text));
				return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Update(hasAccessibility), this.Severity);
			}

			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data, this.Severity);
		}
	}
}