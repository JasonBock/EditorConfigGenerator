namespace EditorConfigGenerator.Core.Statistics
{
	public abstract class Data<TData>
		where TData : Data<TData>
	{
		protected Data(uint totalOccurences) => 
			this.TotalOccurences = totalOccurences;

		public abstract TData Add(TData data);

		public uint TotalOccurences { get; }

		public float Consistency { get; protected set; }
	}
}