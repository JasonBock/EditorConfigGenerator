using EditorConfigGenerator.Core.Statistics;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class SeverityTokenStyle<TData, TNodeInfo, TStyle>
		: TokenStyle<TData, TNodeInfo, TStyle>
		where TData : Data<TData>
		where TNodeInfo : TokenInformation
		where TStyle : SeverityTokenStyle<TData, TNodeInfo, TStyle>
	{
		protected SeverityTokenStyle(TData data, Severity severity = Severity.Error)
			: base(data) => this.Severity = severity;

		public Severity Severity { get; }
	}
}