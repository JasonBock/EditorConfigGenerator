using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class BooleanData
		: Data<BooleanData>, IEquatable<BooleanData>
	{
		public BooleanData() 
			: base(default) { }

		public BooleanData(uint totalOccurences,
			uint trueOccurences, uint falseOccurences)
			: base(totalOccurences)
		{
			this.TrueOccurences = trueOccurences;
			this.FalseOccurences = falseOccurences;
		}

		public BooleanData Update(bool isTrue) =>
			new BooleanData(this.TotalOccurences + 1,
				isTrue ? this.TrueOccurences + 1 : this.TrueOccurences,
				!isTrue ? this.FalseOccurences + 1 : this.FalseOccurences);

		public override BooleanData Add(BooleanData data)
		{
			if(data == null) { throw new ArgumentNullException(nameof(data)); }
			return new BooleanData(
				this.TotalOccurences + data.TotalOccurences,
				this.TrueOccurences + data.TrueOccurences,
				this.FalseOccurences + data.FalseOccurences);
		}

		public bool Equals(BooleanData other)
		{
			var areEqual = false;

			if (other != null)
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.TrueOccurences == other.TrueOccurences &&
					this.FalseOccurences == other.FalseOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as BooleanData);

		public override int GetHashCode() =>
			this.TotalOccurences.GetHashCode() ^
			(this.TrueOccurences.GetHashCode() << 1) ^
			(this.FalseOccurences.GetHashCode() << 2);

		public override string ToString() => $"{this.TotalOccurences}, {this.TrueOccurences}, {this.FalseOccurences}";

		public static bool operator ==(BooleanData a, BooleanData b)
		{
			var areEqual = false;

			if (object.ReferenceEquals(a, b))
			{
				areEqual = true;
			}

			if ((object)a != null && (object)b != null)
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
