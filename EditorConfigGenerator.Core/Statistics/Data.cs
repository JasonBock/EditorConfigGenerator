namespace EditorConfigGenerator.Core.Statistics
{
	public abstract class Data
	{
		protected Data(uint totalOccurences) => 
			this.TotalOccurences = totalOccurences;

		public uint TotalOccurences { get; }
	}
}
