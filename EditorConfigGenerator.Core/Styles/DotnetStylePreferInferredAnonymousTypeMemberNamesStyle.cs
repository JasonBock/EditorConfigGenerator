using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetStylePreferInferredAnonymousTypeMemberNamesStyle
		: SeverityStyle<BooleanData, AnonymousObjectMemberDeclaratorSyntax, NodeInformation<AnonymousObjectMemberDeclaratorSyntax>, DotnetStylePreferInferredAnonymousTypeMemberNamesStyle>
	{
		public DotnetStylePreferInferredAnonymousTypeMemberNamesStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetStylePreferInferredAnonymousTypeMemberNamesStyle Add(DotnetStylePreferInferredAnonymousTypeMemberNamesStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetStylePreferInferredAnonymousTypeMemberNamesStyle(this.Data.Add(style.Data), this.Severity);
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_style_prefer_inferred_anonymous_type_member_names = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetStylePreferInferredAnonymousTypeMemberNamesStyle Update(NodeInformation<AnonymousObjectMemberDeclaratorSyntax> information)
		{
			if (information == null) { throw new ArgumentNullException(nameof(information)); }

			var node = information.Node;

			if (!node.ContainsDiagnostics)
			{
				return new DotnetStylePreferInferredAnonymousTypeMemberNamesStyle(
					this.Data.Update(!node.ChildNodes().Any(_ => _.Kind() == SyntaxKind.NameEquals)), this.Severity);
			}

			return new DotnetStylePreferInferredAnonymousTypeMemberNamesStyle(this.Data, this.Severity);
		}
	}
}