using System;
using System.ComponentModel;

namespace EditorConfigGenerator.Core.Extensions
{
	public static class EnumExtensions
	{
		// Lifted from:
		// https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute
		public static string GetDescription(this Enum value)
		{
			var field = value.GetType().GetField(value.ToString());
			var attribute = Attribute.GetCustomAttribute(
				field, typeof(DescriptionAttribute)) as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}
	}
}
