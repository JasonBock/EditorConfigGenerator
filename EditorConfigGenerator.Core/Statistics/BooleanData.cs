using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class BooleanData
		: Data<BooleanData>
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

		public uint FalseOccurences { get; }
		public uint TrueOccurences { get; }
	}
}
