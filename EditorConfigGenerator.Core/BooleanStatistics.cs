namespace EditorConfigGenerator.Core
{
	public sealed class BooleanStatistics
		: Statistics
	{
		public BooleanStatistics(uint totalOccurences,
			uint trueOccurences, uint falseOccurences)
			: base(totalOccurences)
		{
			this.TrueOccurences = trueOccurences;
			this.FalseOccurences = falseOccurences;
		}

		public BooleanStatistics Update(bool isTrue) =>
			new BooleanStatistics(this.TotalOccurences + 1,
				isTrue ? this.TrueOccurences + 1 : this.TrueOccurences,
				!isTrue ? this.FalseOccurences + 1 : this.FalseOccurences);

		public uint FalseOccurences { get; }
		public uint TrueOccurences { get; }
	}
}
