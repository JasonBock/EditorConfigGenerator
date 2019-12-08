using System;
using System.Runtime.Serialization;

namespace EditorConfigGenerator.Core.Statistics
{
	[Serializable]
	public sealed class InvalidOccurenceValuesException
		: Exception
	{
		public InvalidOccurenceValuesException() { }

		public InvalidOccurenceValuesException(string message) : base(message) { }

		public InvalidOccurenceValuesException(string message, Exception inner) : base(message, inner) { }
		
		private InvalidOccurenceValuesException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}