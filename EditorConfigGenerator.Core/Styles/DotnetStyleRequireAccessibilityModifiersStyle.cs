using EditorConfigGenerator.Core.Extensions;
using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStyleRequireAccessibilityModifiersStyle
		: SeverityNodeStyle<AccessibilityModifierData, MemberDeclarationSyntax, NodeInformation<MemberDeclarationSyntax>, DotnetStyleRequireAccessibilityModifiersStyle>
	{
		private const string AccessibilityModifierInternal = "internal";
		private const string AccessibilityModifierPrivate = "private";
		private const string AccessibilityModifierProtected = "protected";
		private const string AccessibilityModifierPublic = "public";
		public const string Setting = "dotnet_style_require_accessibility_modifiers";

		private static readonly string[] AccessibilityModifiers = 
			new[] { AccessibilityModifierPublic, AccessibilityModifierProtected, AccessibilityModifierInternal, AccessibilityModifierPrivate };

		public DotnetStyleRequireAccessibilityModifiersStyle(AccessibilityModifierData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStyleRequireAccessibilityModifiersStyle Add(DotnetStyleRequireAccessibilityModifiersStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting(DotnetStyleRequireAccessibilityModifiersStyle.Setting, this.Severity);

		private static bool AreModifiersDefault(MemberDeclarationSyntax node, List<SyntaxToken> modifiers)
		{
			var parent = node.FindParent<TypeDeclarationSyntax>();

			if(modifiers.Count == 1)
			{
				if (node is ClassDeclarationSyntax || node is StructDeclarationSyntax || node is InterfaceDeclarationSyntax ||
					node is DelegateDeclarationSyntax)
				{
					return parent is null ? modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPrivate :
						modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierInternal;
				}
				else if (node is EnumDeclarationSyntax)
				{
					return parent is null ? modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPublic :
						modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierInternal;
				}
				else
				{
					if (node is ConstructorDeclarationSyntax || node is MethodDeclarationSyntax || node is PropertyDeclarationSyntax ||
						node is EventDeclarationSyntax || node is FieldDeclarationSyntax)
					{
						return modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPrivate;
					}
					else
					{
						return false;
					}
				}
			}
			else
			{
				return false;
			}
		}

		public override DotnetStyleRequireAccessibilityModifiersStyle Update(NodeInformation<MemberDeclarationSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var modifiers = node.ChildTokens().Where(
					_ => DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifiers.Contains(_.Text)).ToList();

				if(modifiers.Count == 0)
				{
					return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Update(AccessibilityModifierDataOccurence.NotProvided), this.Severity);
				}
				else
				{
					if(DotnetStyleRequireAccessibilityModifiersStyle.AreModifiersDefault(node, modifiers))
					{
						return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Update(AccessibilityModifierDataOccurence.ProvidedDefault), this.Severity);
					}
					else
					{
						return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Update(AccessibilityModifierDataOccurence.ProvidedNotDefault), this.Severity);
					}
				}
			}

			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data, this.Severity);
		}
	}
}