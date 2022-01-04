﻿using Microsoft.CodeAnalysis;

namespace EditorConfigGenerator.Core.Extensions;

internal static class SpecialNameExtensions
{
	internal static bool IsPredefinedType(this SpecialType @this)
	{
		switch (@this)
		{
			case SpecialType.System_Boolean:
			case SpecialType.System_Byte:
			case SpecialType.System_SByte:
			case SpecialType.System_Char:
			case SpecialType.System_Decimal:
			case SpecialType.System_Double:
			case SpecialType.System_Single:
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
			case SpecialType.System_Object:
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
			case SpecialType.System_String:
				return true;
			default:
				return false;
		}
	}
}