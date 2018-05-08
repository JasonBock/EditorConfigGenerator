using EditorConfigGenerator.Core.Statistics;
using System;

namespace EditorConfigGenerator.Core.Styles
{
	public abstract class TokenStyle<TData, TTokenInfo, TStyle>
		where TData : Data<TData>
		where TTokenInfo : TokenInformation
		where TStyle : TokenStyle<TData, TTokenInfo, TStyle>
	{
		protected TokenStyle(TData data) => 
			this.Data = data ?? throw new ArgumentNullException(nameof(data));

		public abstract TStyle Add(TStyle style);

		public abstract string GetSetting();

		public abstract TStyle Update(TTokenInfo information);

		public TData Data { get; }
	}
}
