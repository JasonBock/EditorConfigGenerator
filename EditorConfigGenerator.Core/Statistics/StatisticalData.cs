namespace EditorConfigGenerator.Core.Statistics;

public abstract class StatisticalData<TData>
	where TData : StatisticalData<TData>
{
	protected StatisticalData(uint totalOccurences, float consistency) =>
		(this.TotalOccurences, this.Consistency) = (totalOccurences, consistency);

	public abstract TData Add(TData data);

	public uint TotalOccurences { get; }

	public float Consistency { get; }
}