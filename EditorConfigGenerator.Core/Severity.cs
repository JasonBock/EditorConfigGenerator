using System.ComponentModel;

namespace EditorConfigGenerator.Core
{
	public enum Severity
	{
		[Description("none")]
		None,
		[Description("suggestion")]
		Suggestion,
		[Description("warning")]
		Warning,
		[Description("error")]
		Error
	}
}
