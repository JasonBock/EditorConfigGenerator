using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class BooleanData
		: Data<BooleanData>, IEquatable<BooleanData?>
	{
		public BooleanData() 
			: base(default) { }

		public BooleanData(uint totalOccurences,
			uint trueOccurences, uint falseOccurences)
			: base(totalOccurences)
		{
			this.TrueOccurences = trueOccurences;
			this.FalseOccurences = falseOccurences;
			this.Consistency = (float)Math.Abs(trueOccurences - falseOccurences) / totalOccurences;
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
			HashCode.Combine(this.TotalOccurences, this.TrueOccurences, this.FalseOccurences);

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
