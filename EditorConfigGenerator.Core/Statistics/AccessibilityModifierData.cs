using System;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class AccessibilityModifierData
		: Data<AccessibilityModifierData>, IEquatable<AccessibilityModifierData?>
	{
		private const string ValueAlways = "always";
		private const string ValueNever = "never";
		private const string ValueOmitIfDefault = "omit_if_default";

		public AccessibilityModifierData()
			: base(default) { }

		public AccessibilityModifierData(uint totalOccurences,
			uint notProvidedOccurences, uint providedDefaultOccurences, uint providedNotDefaultOccurences)
			: base(totalOccurences) =>
			(this.NotProvidedOccurences, this.ProvidedDefaultOccurences, this.ProvidedNotDefaultOccurences) =
				(notProvidedOccurences, providedDefaultOccurences, providedNotDefaultOccurences);

		public AccessibilityModifierData Update(AccessibilityModifierDataOccurence occurence) =>
			new AccessibilityModifierData(this.TotalOccurences + 1,
				occurence == AccessibilityModifierDataOccurence.NotProvided ? this.NotProvidedOccurences + 1 : this.NotProvidedOccurences,
				occurence == AccessibilityModifierDataOccurence.ProvidedDefault ? this.ProvidedDefaultOccurences + 1 : this.ProvidedDefaultOccurences,
				occurence == AccessibilityModifierDataOccurence.ProvidedNotDefault ? this.ProvidedNotDefaultOccurences + 1 : this.ProvidedNotDefaultOccurences);

		public string GetSetting(string name, Severity severity)
		{
			/*
			Case "NotProvided" - If you don't put the modifier in, it means you prefer "never"
			Case "ProvidedNotDefault" - If you put the modifier in and it is NOT the default, it means you like "omit_if_default" more.
			Case "ProvidedDefault" - If you put the modifier in and it is the default, it means you like "omit_if_default" less.

			3 NotProvided => "never"
			3 NotProvided, 1 ProvidedNotDefault => "never"
			3 NotProvided, 3 ProvidedDefault => "always"

			4 NotProvided, 3 ProvidedDefault, 5 ProvidedNotDefault 

			If ProvidedNotDefault is largest (winning ties)
				If ProvidedDefault >= NotProvided, "always"
				Else "omit_if_default"
			Else If ProvidedDefault >= NotProvided
				"always"
			Else
				// 10 NotProvideds, 9 ProvidedNotDefault, 8 ProvidedDefault
				If NotProvided >= (ProvidedNotDefault + ProvidedDefault)
					"never"
				Else
					If ProvidedDefault >= ProvidedNotDefault
						"always"
					Else
						"omit_if_default"
			*/

			if (this.TotalOccurences > 0)
			{
				string value;

				if (this.ProvidedNotDefaultOccurences >= this.ProvidedDefaultOccurences &&
					this.ProvidedNotDefaultOccurences >= this.NotProvidedOccurences)
				{
					value = this.ProvidedDefaultOccurences >= this.NotProvidedOccurences ?
						AccessibilityModifierData.ValueAlways :
						AccessibilityModifierData.ValueOmitIfDefault;
				}
				else if (this.ProvidedDefaultOccurences >= this.NotProvidedOccurences)
				{
					value = AccessibilityModifierData.ValueAlways;
				}
				else
				{
					if (this.NotProvidedOccurences >= (this.ProvidedNotDefaultOccurences + this.ProvidedDefaultOccurences))
					{
						value = AccessibilityModifierData.ValueNever;
					}
					else
					{
						value = this.ProvidedDefaultOccurences >= this.ProvidedNotDefaultOccurences ?
							AccessibilityModifierData.ValueAlways :
							AccessibilityModifierData.ValueOmitIfDefault;
					}
				}

				return $"{name} = {value}:{severity.GetDescription()}";
			}
			else
			{
				return string.Empty;
			}
		}

		public override AccessibilityModifierData Add(AccessibilityModifierData data)
		{
			if (data is null) { throw new ArgumentNullException(nameof(data)); }
			return new AccessibilityModifierData(
				this.TotalOccurences + data.TotalOccurences,
				this.NotProvidedOccurences + data.NotProvidedOccurences,
				this.ProvidedDefaultOccurences + data.ProvidedDefaultOccurences,
				this.ProvidedNotDefaultOccurences + data.ProvidedNotDefaultOccurences);
		}

		public bool Equals(AccessibilityModifierData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.NotProvidedOccurences == other.NotProvidedOccurences &&
					this.ProvidedDefaultOccurences == other.ProvidedDefaultOccurences &&
					this.ProvidedNotDefaultOccurences == other.ProvidedNotDefaultOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as AccessibilityModifierData);

		public override int GetHashCode() =>
			HashCode.Combine(this.TotalOccurences, this.NotProvidedOccurences, this.ProvidedDefaultOccurences, this.ProvidedNotDefaultOccurences);

		public override string ToString() =>
			$"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.NotProvidedOccurences)} = {this.NotProvidedOccurences}, {nameof(this.ProvidedDefaultOccurences)} = {this.ProvidedDefaultOccurences}, {nameof(this.ProvidedNotDefaultOccurences)} = {this.ProvidedNotDefaultOccurences}";

		public static bool operator ==(AccessibilityModifierData a, AccessibilityModifierData b)
		{
			var areEqual = false;

			if (object.ReferenceEquals(a, b))
			{
				areEqual = true;
			}

			if (a is { } && b is { })
			{
				areEqual = a.Equals(b);
			}

			return areEqual;
		}

		public static bool operator !=(AccessibilityModifierData a, AccessibilityModifierData b) =>
			!(a == b);

		public uint NotProvidedOccurences { get; }
		public uint ProvidedDefaultOccurences { get; }
		public uint ProvidedNotDefaultOccurences { get; }
	}
}
