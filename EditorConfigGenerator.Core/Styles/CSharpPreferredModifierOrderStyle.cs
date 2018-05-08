using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class CSharpPreferredModifierOrderStyle
		: NodeStyle<ModifierData, MemberDeclarationSyntax, NodeInformation<MemberDeclarationSyntax>, CSharpPreferredModifierOrderStyle>
	{
		public CSharpPreferredModifierOrderStyle(ModifierData data)
			: base(data) { }

		public override CSharpPreferredModifierOrderStyle Add(CSharpPreferredModifierOrderStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new CSharpPreferredModifierOrderStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var visibilityFrequency = this.Data.VisibilityModifiers.Sum(_ => _.Value.frequency);
				var otherFrequency = this.Data.OtherModifiers.Sum(_ => _.Value.frequency);

				var greaterModifiers = visibilityFrequency > otherFrequency ?
					this.Data.VisibilityModifiers : this.Data.OtherModifiers;
				var lesserModifiers = visibilityFrequency > otherFrequency ?
					this.Data.OtherModifiers : this.Data.VisibilityModifiers;

				var modifierOrder = greaterModifiers
					.Where(_ => _.Value.frequency > 0u)
					.OrderByDescending(_ => _.Value.frequency)
					.ThenByDescending(_ => _.Value.weight).Select(_ => _.Key)
					.Union(lesserModifiers
						.Where(_ => _.Value.frequency > 0u)
						.OrderByDescending(_ => _.Value.frequency)
						.ThenByDescending(_ => _.Value.weight).Select(_ => _.Key));
				return $"csharp_preferred_modifier_order = {string.Join(",", modifierOrder)}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override CSharpPreferredModifierOrderStyle Update(NodeInformation<MemberDeclarationSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				return new CSharpPreferredModifierOrderStyle(
					this.Data.Update(node.ChildTokens()
						.Where(_ => ((SyntaxKind)_.RawKind).ToString().Contains("Keyword"))
						.Select(_ => _.ValueText).ToImmutableList()));
			}

			return new CSharpPreferredModifierOrderStyle(this.Data);
		}
	}
}