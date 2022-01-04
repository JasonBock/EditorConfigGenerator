using SCM = System.ComponentModel;

namespace EditorConfigGenerator.Core.Tests.Extensions;

public enum TestWithDescriptions
{
	[SCM.Description("number one")]
	One,
	[SCM.Description("number two")]
	Two,
}