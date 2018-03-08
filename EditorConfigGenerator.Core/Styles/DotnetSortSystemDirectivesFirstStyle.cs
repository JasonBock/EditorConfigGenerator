using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Styles
{
	public sealed class DotnetSortSystemDirectivesFirstStyle
		: SeverityStyle<BooleanData, SyntaxNode, DotnetSortSystemDirectivesFirstStyle>
	{
		public DotnetSortSystemDirectivesFirstStyle(BooleanData data, Severity severity = Severity.Error)
			: base(data, severity) { }

		public override DotnetSortSystemDirectivesFirstStyle Add(DotnetSortSystemDirectivesFirstStyle style)
		{
			if (style == null) { throw new ArgumentNullException(nameof(style)); }
			return new DotnetSortSystemDirectivesFirstStyle(this.Data.Add(style.Data));
		}

		public override string GetSetting()
		{
			if (this.Data.TotalOccurences > 0)
			{
				var value = this.Data.TrueOccurences >= this.Data.FalseOccurences ? "true" : "false";
				return $"dotnet_sort_system_directives_first = {value}:{this.Severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override DotnetSortSystemDirectivesFirstStyle Update(SyntaxNode node)
		{
			if (node == null) { throw new ArgumentNullException(nameof(node)); }

			if(!node.ContainsDiagnostics)
			{
				var usingNodes = node.DescendantNodes().Where(_ => _.IsKind(SyntaxKind.UsingDirective))
					.Select(_ => _ as UsingDirectiveSyntax).ToImmutableList();

				if(usingNodes.Count > 1)
				{
					var systemSortBreakIndex = 0;
					var foundBreakIndex = false;

					for (var i = 0; i < usingNodes.Count; i++)
					{
						var usingNode = usingNodes[i];

						if (!usingNode.Name.ToString().StartsWith("System"))
						{
							foundBreakIndex = true;
							systemSortBreakIndex = i;
						}
						else if(foundBreakIndex)
						{
							return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(false));
						}
					}

					var systemUsingNodes = usingNodes.GetRange(
						0, foundBreakIndex ? systemSortBreakIndex : usingNodes.Count);

					if(systemUsingNodes.Count > 1)
					{
						for(var i = 1; i < systemUsingNodes.Count; i++)
						{
							if(systemUsingNodes[i - 1].Name.ToString().CompareTo(
								systemUsingNodes[i - 1].Name.ToString()) > 0)
							{
								return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(false));
							}
						}

						return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(true));
					}
					else if(systemUsingNodes.Count == 1)
					{
						return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(true));
					}
					else
					{
						return new DotnetSortSystemDirectivesFirstStyle(this.Data);
					}
				}

				return new DotnetSortSystemDirectivesFirstStyle(this.Data);
			}

			return new DotnetSortSystemDirectivesFirstStyle(this.Data);
		}
	}
}
