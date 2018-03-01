namespace EditorConfigGenerator.Core
{
	public abstract class Statistics
	{
		protected Statistics(uint totalOccurences) => 
			this.TotalOccurences = totalOccurences;

		public uint TotalOccurences { get; }
	}
}
