namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class BooleanData
		: Data
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

		public uint FalseOccurences { get; }
		public uint TrueOccurences { get; }
	}
}
