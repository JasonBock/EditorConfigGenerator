using SCM = System.ComponentModel;

namespace EditorConfigGenerator.Core.Tests.Extensions
{
	public enum TestDescription
	{
		[SCM.Description("one")]
		One,
		[SCM.Description("two")]
		Two,
	}
}
