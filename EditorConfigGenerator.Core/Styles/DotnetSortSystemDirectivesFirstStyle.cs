﻿using EditorConfigGenerator.Core.Statistics;
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
					.Select(_ => _ as UsingDirectiveSyntax).ToImmutableArray();

				if(usingNodes.Length > 1)
				{
					var foundBreakIndex = false;

					for (var i = 0; i < usingNodes.Length; i++)
					{
						var usingNode = usingNodes[i];

						if (!usingNode.Name.ToString().StartsWith("System"))
						{
							foundBreakIndex = true;
						}
						else if(foundBreakIndex)
						{
							return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(false));
						}
					}

					var systemUsingNodes = usingNodes.Where(_ => _.Name.ToString().StartsWith("System")).ToImmutableArray();

					if (systemUsingNodes.Length > 1)
					{
						for(var i = 1; i < systemUsingNodes.Length; i++)
						{
							if(systemUsingNodes[i - 1].Name.ToString().CompareTo(
								systemUsingNodes[i].Name.ToString()) > 0)
							{
								return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(false));
							}
						}

						return new DotnetSortSystemDirectivesFirstStyle(this.Data.Update(true));
					}
					else if(systemUsingNodes.Length == 1)
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
