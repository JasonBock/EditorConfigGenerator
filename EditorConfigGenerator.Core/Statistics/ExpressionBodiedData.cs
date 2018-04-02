using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ExpressionBodiedData
		: Data<ExpressionBodiedData>
	{
		public ExpressionBodiedData()
			: base(default) { }

		public ExpressionBodiedData(uint totalOccurences,
			uint arrowSingleLineOccurences, uint arrowMultiLineOccurences,
			uint blockOccurences)
			: base(totalOccurences)
		{
			this.ArrowSingleLineOccurences = arrowSingleLineOccurences;
			this.ArrowMultiLineOccurences = arrowMultiLineOccurences;
			this.BlockOccurences = blockOccurences;
		}

		public string GetSetting(string name, Severity severity)
		{
			if(this.TotalOccurences > 0)
			{
				var value = string.Empty;
				var arrowCount = this.ArrowSingleLineOccurences + this.ArrowMultiLineOccurences;
				var blockCount = this.BlockOccurences;

				if (blockCount == 0)
				{
					value = "true";
				}
				else if (arrowCount == 0)
				{
					value = "false";
				}
				else
				{
					if (arrowCount > blockCount)
					{
						value = this.ArrowMultiLineOccurences > this.ArrowSingleLineOccurences ?
							"when_on_single_line" : "true";
					}
					else
					{
						value = "false";
					}
				}

				return $"{name} = {value}:{severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public ExpressionBodiedData Update(ExpressionBodiedDataOccurence occurence) =>
			new ExpressionBodiedData(this.TotalOccurences + 1,
				occurence == ExpressionBodiedDataOccurence.ArrowSingleLine ? this.ArrowSingleLineOccurences + 1 : this.ArrowSingleLineOccurences,
				occurence == ExpressionBodiedDataOccurence.ArrowMultiLine ? this.ArrowMultiLineOccurences + 1 : this.ArrowMultiLineOccurences,
				occurence == ExpressionBodiedDataOccurence.Block ? this.BlockOccurences + 1 : this.BlockOccurences);

		public override ExpressionBodiedData Add(ExpressionBodiedData data)
		{
			if (data == null) { throw new ArgumentNullException(nameof(data)); }
			return new ExpressionBodiedData(
				this.TotalOccurences + data.TotalOccurences,
				this.ArrowSingleLineOccurences + data.ArrowSingleLineOccurences,
				this.ArrowMultiLineOccurences + data.ArrowMultiLineOccurences,
				this.BlockOccurences + data.BlockOccurences);
		}

		public uint ArrowSingleLineOccurences { get; }
		public uint ArrowMultiLineOccurences { get; }
		public uint BlockOccurences { get; }
	}
}