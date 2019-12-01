using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public class A { }
	class C 
	{
		void Foo() { }
		private void Bar() { }
	}
	internal class B { }

	/*

	Case "Never" - If you don't put the modifier in, it means you prefer "never"
	Case "AlwaysNotDefault" - If you put the modifier in and it is NOT the default, it means you like "omit_if_default" more.
	Case "AlwaysDefault" - If you put the modifier in and it is the default, it means you like "omit_if_default" less.

	3 Never => "never"
	3 Never, 1 AlwaysNotDefault => "never"
	3 Never, 3 AlwaysDefault => "always"

	4 Never, 3 AlwaysDefault, 5 AlwaysNotDefault 

	If AlwaysNotDefault is largest (winning ties)
		If AlwaysDefault >= Never, "always"
		Else "omit_if_default"
	Else If AlwaysDefault >= Never
		"always"
	Else
		10 Nevers, 9 AlwaysNotDefault, 8 AlwaysDefault
		If Nevers >= (AlwaysNotDefault + AlwaysDefault)
			"never"
		Else
			If AlwaysDefault >= AlwaysNotDefault
				"always"
			Else
				"omit_if_default"
	*/

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