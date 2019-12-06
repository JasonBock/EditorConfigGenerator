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
			if (style is null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting() =>
			this.Data.GetSetting(DotnetStyleRequireAccessibilityModifiersStyle.Setting, this.Severity);

		private static (AccessibilityModifierDataOccurence, bool) GetOccurence(MemberDeclarationSyntax node, List<SyntaxToken> modifiers)
		{
			var parent = node.FindParent<TypeDeclarationSyntax>();
			var isFromPublicInterfaceMember = false;

			if (modifiers.Count == 0)
			{
				isFromPublicInterfaceMember = parent is InterfaceDeclarationSyntax parentInterface &&
					parentInterface.ChildTokens().Any(
						_ => DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifiers.Contains(
							DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPublic));
				return (AccessibilityModifierDataOccurence.NotProvided, isFromPublicInterfaceMember);
			}
			else if(modifiers.Count == 1)
			{
				var isDefault = false;

				if (node is ClassDeclarationSyntax || node is StructDeclarationSyntax || node is InterfaceDeclarationSyntax ||
					node is DelegateDeclarationSyntax)
				{
					isDefault = parent is null ? modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierInternal :
						modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPrivate;
				}
				else if (node is EnumDeclarationSyntax)
				{
					isDefault = modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPublic;
				}
				else
				{
					if (node is ConstructorDeclarationSyntax || node is MethodDeclarationSyntax || node is PropertyDeclarationSyntax ||
						node is EventFieldDeclarationSyntax || node is FieldDeclarationSyntax)
					{
						isDefault = modifiers[0].Text == DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPrivate;
						isFromPublicInterfaceMember = parent is InterfaceDeclarationSyntax parentInterface &&
							parentInterface.ChildTokens().Any(
								_ => DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifiers.Contains(
									DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifierPublic));
					}
				}

				return (isDefault ? AccessibilityModifierDataOccurence.ProvidedDefault : AccessibilityModifierDataOccurence.ProvidedNotDefault, isFromPublicInterfaceMember);
			}
			else
			{
				return (AccessibilityModifierDataOccurence.ProvidedNotDefault, isFromPublicInterfaceMember);
			}
		}

		public override DotnetStyleRequireAccessibilityModifiersStyle Update(NodeInformation<MemberDeclarationSyntax> information)
		{
			if (information is null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				var modifiers = node.ChildTokens().Where(
					_ => DotnetStyleRequireAccessibilityModifiersStyle.AccessibilityModifiers.Contains(_.Text)).ToList();
				var (occurence, isFromPublicInterfaceMember) = DotnetStyleRequireAccessibilityModifiersStyle.GetOccurence(node, modifiers);
				return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data.Update(occurence, isFromPublicInterfaceMember), this.Severity);
			}

			return new DotnetStyleRequireAccessibilityModifiersStyle(this.Data, this.Severity);
		}
	}
}