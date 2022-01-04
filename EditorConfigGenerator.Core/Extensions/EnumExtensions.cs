using System.ComponentModel;

namespace EditorConfigGenerator.Core.Extensions;

public static class EnumExtensions
{
   // Lifted from:
   // https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute
   public static string GetDescription(this Enum value)
   {
	  if (value is null)
	  {
		 throw new ArgumentNullException(nameof(value));
	  }

	  return Attribute.GetCustomAttribute(
		  value.GetType().GetField(value.ToString())!, typeof(DescriptionAttribute)) is not DescriptionAttribute attribute ?
		  value.ToString() : attribute.Description;
   }
}