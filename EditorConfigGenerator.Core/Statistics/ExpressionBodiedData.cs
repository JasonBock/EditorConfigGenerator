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
			uint blockSingleLineOccurences, uint blockMultiLineOccurences)
			: base(totalOccurences)
		{
			this.ArrowSingleLineOccurences = arrowSingleLineOccurences;
			this.ArrowMultiLineOccurences = arrowMultiLineOccurences;
			this.BlockSingleLineOccurences = blockSingleLineOccurences;
			this.BlockMultiLineOccurences = blockMultiLineOccurences;
		}

		public string GetSetting(string name, Severity severity)
		{
			if(this.TotalOccurences > 0)
			{
				var value = string.Empty;
				var arrowCount = this.ArrowSingleLineOccurences + this.ArrowMultiLineOccurences;
				var blockCount = this.BlockSingleLineOccurences + this.BlockMultiLineOccurences;

				if(blockCount == 0)
				{
					value = "true";
				}
				else if (arrowCount == 0)
				{
					value = "false";
				}
				else if (arrowCount > blockCount)
				{
					if(this.BlockMultiLineOccurences > this.ArrowMultiLineOccurences)
					{
						value = "when_on_single_line";
					}
					else
					{
						value = "true";
					}
				}
				else
				{
					value = "false";
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
				occurence == ExpressionBodiedDataOccurence.BlockSingleLine ? this.BlockSingleLineOccurences + 1 : this.BlockSingleLineOccurences,
				occurence == ExpressionBodiedDataOccurence.BlockMultiLine ? this.BlockMultiLineOccurences + 1 : this.BlockMultiLineOccurences);

		public override ExpressionBodiedData Add(ExpressionBodiedData data)
		{
			if (data == null) { throw new ArgumentNullException(nameof(data)); }
			return new ExpressionBodiedData(
				this.TotalOccurences + data.TotalOccurences,
				this.ArrowSingleLineOccurences + data.ArrowSingleLineOccurences,
				this.ArrowMultiLineOccurences + data.ArrowMultiLineOccurences,
				this.BlockSingleLineOccurences + data.BlockSingleLineOccurences,
				this.BlockMultiLineOccurences + data.BlockMultiLineOccurences);
		}

		public uint ArrowSingleLineOccurences { get; }
		public uint ArrowMultiLineOccurences { get; }
		public uint BlockSingleLineOccurences { get; }
		public uint BlockMultiLineOccurences { get; }
	}
}