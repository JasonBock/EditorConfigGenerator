namespace EditorConfigGenerator.Core.Statistics
{
	public abstract class Data<TData>
		where TData : Data<TData>
	{
		protected Data(uint totalOccurences, float consistency) => 
			(this.TotalOccurences, this.Consistency) = (totalOccurences, consistency);

		public abstract TData Add(TData data);

		public uint TotalOccurences { get; }

		public float Consistency { get; }
	}
}