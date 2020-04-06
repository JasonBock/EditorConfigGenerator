using System;
using System.Collections.Generic;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class BooleanData
		: Data<BooleanData>, IEquatable<BooleanData?>
	{
		public BooleanData() 
			: base(default, default) { }

		public BooleanData(uint totalOccurences,
			uint trueOccurences, uint falseOccurences)
			: base(totalOccurences, new List<uint> { trueOccurences, falseOccurences }.GetConsistency(totalOccurences))
		{
			if(trueOccurences + falseOccurences != totalOccurences)
			{
				throw new InvalidOccurenceValuesException(
					$"{trueOccurences} ({nameof(trueOccurences)}) + {falseOccurences} ({nameof(falseOccurences)}) != {totalOccurences} ({nameof(totalOccurences)})");
			}

			this.TrueOccurences = trueOccurences;
			this.FalseOccurences = falseOccurences;
		}

		public BooleanData Update(bool isTrue) =>
			new BooleanData(this.TotalOccurences + 1,
				isTrue ? this.TrueOccurences + 1 : this.TrueOccurences,
				!isTrue ? this.FalseOccurences + 1 : this.FalseOccurences);

		public override BooleanData Add(BooleanData data)
		{
			if(data is null) { throw new ArgumentNullException(nameof(data)); }
			return new BooleanData(
				this.TotalOccurences + data.TotalOccurences,
				this.TrueOccurences + data.TrueOccurences,
				this.FalseOccurences + data.FalseOccurences);
		}

		public bool Equals(BooleanData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.TrueOccurences == other.TrueOccurences &&
					this.FalseOccurences == other.FalseOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as BooleanData);

		public override int GetHashCode() =>
#if NETSTANDARD2_1
		System.HashCode.Combine(this.TotalOccurences, this.TrueOccurences, this.FalseOccurences);
#else
		HashCode.Combine(this.TotalOccurences, this.TrueOccurences, this.FalseOccurences);
#endif


		public override string ToString() => 
			$"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.TrueOccurences)} = {this.TrueOccurences}, {nameof(this.FalseOccurences)} = {this.FalseOccurences}";

		public static bool operator ==(BooleanData a, BooleanData b)
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

		public static bool operator !=(BooleanData a, BooleanData b) =>
			!(a == b);

		public uint FalseOccurences { get; }
		public uint TrueOccurences { get; }
	}
}
