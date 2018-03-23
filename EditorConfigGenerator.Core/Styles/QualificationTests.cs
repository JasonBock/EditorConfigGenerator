namespace EditorConfigGenerator.Core.Styles
{
	public class QualificationTests
	{
		public void CallThis() { }
	}

	public static class UseItExtensions
	{
		public static void ExtendIt(this UseIt @this) { }
	}

	public class UseIt
	{
		public static void StaticCall() { }

		public void CallingIt()
		{
			var qualifiedToString = this.ToString();
			var unqualifiedToString = ToString();

			StaticCall();
			UseIt.StaticCall();

			var qt = new QualificationTests();
			qt.CallThis();

			this.ExtendIt();
		}
	}
}
