using System;
using System.Collections.Generic;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ExpressionBodiedData
		: Data<ExpressionBodiedData>, IEquatable<ExpressionBodiedData?>
	{
		public ExpressionBodiedData()
			: base(default, default) { }

		public ExpressionBodiedData(uint totalOccurences,
			uint arrowSingleLineOccurences, uint arrowMultiLineOccurences,
			uint blockOccurences)
			: base(totalOccurences, new List<uint> { arrowSingleLineOccurences + arrowMultiLineOccurences, blockOccurences }.GetConsistency(totalOccurences))
		{
			if(arrowSingleLineOccurences + arrowMultiLineOccurences + blockOccurences != totalOccurences)
			{
				throw new InvalidOccurenceValuesException(
					$"{arrowSingleLineOccurences} ({nameof(arrowSingleLineOccurences)}) + {arrowMultiLineOccurences} ({nameof(blockOccurences)}) + {blockOccurences} ({nameof(blockOccurences)}) != {totalOccurences} ({nameof(totalOccurences)})");
			}

			this.ArrowSingleLineOccurences = arrowSingleLineOccurences;
			this.ArrowMultiLineOccurences = arrowMultiLineOccurences;
			this.BlockOccurences = blockOccurences;
		}

		public string GetSetting(string name, Severity severity)
		{
			if(this.TotalOccurences > 0)
			{
				var arrowCount = this.ArrowSingleLineOccurences + this.ArrowMultiLineOccurences;
				var blockCount = this.BlockOccurences;

				string value;
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
			if (data is null) { throw new ArgumentNullException(nameof(data)); }
			return new ExpressionBodiedData(
				this.TotalOccurences + data.TotalOccurences,
				this.ArrowSingleLineOccurences + data.ArrowSingleLineOccurences,
				this.ArrowMultiLineOccurences + data.ArrowMultiLineOccurences,
				this.BlockOccurences + data.BlockOccurences);
		}

		public bool Equals(ExpressionBodiedData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.ArrowSingleLineOccurences == other.ArrowSingleLineOccurences &&
					this.ArrowMultiLineOccurences == other.ArrowMultiLineOccurences &&
					this.BlockOccurences == other.BlockOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as ExpressionBodiedData);

		public override int GetHashCode() =>
			this.TotalOccurences.GetHashCode() ^
			(this.ArrowSingleLineOccurences.GetHashCode() << 1) ^
			(this.ArrowMultiLineOccurences.GetHashCode() << 2) ^
			(this.BlockOccurences.GetHashCode() << 3);

		public override string ToString() => 
			$"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.ArrowSingleLineOccurences)} = {this.ArrowSingleLineOccurences}, {nameof(this.ArrowMultiLineOccurences)} = {this.ArrowMultiLineOccurences}, {nameof(this.BlockOccurences)} = {this.BlockOccurences}";

		public static bool operator ==(ExpressionBodiedData a, ExpressionBodiedData b)
		{
			var areEqual = false;

			if (object.ReferenceEquals(a, b))
			{
				areEqual = true;
			}

			if (a is { } && b is { })
			{
				areEqual = a.Equals(b);
			}

			return areEqual;
		}

		public static bool operator !=(ExpressionBodiedData a, ExpressionBodiedData b) =>
			!(a == b);

		public uint ArrowSingleLineOccurences { get; }
		public uint ArrowMultiLineOccurences { get; }
		public uint BlockOccurences { get; }
	}
}