using System;
using System.Collections.Generic;
using static EditorConfigGenerator.Core.Extensions.EnumExtensions;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class AccessibilityModifierData
		: Data<AccessibilityModifierData>, IEquatable<AccessibilityModifierData?>
	{
		private const string ValueAlways = "always";
		private const string ValueForNonInterfaceMembers = "for_non_interface_members";
		private const string ValueNever = "never";
		private const string ValueOmitIfDefault = "omit_if_default";

		public AccessibilityModifierData()
			: base(default, default) { }

		public AccessibilityModifierData(uint totalOccurences,
			uint notProvidedOccurences, uint providedDefaultOccurences, uint providedNotDefaultOccurences,
			uint notProvidedForPublicInterfaceMembersOccurences, uint providedForPublicInterfaceMembersOccurences)
			: base(totalOccurences, new List<uint> { notProvidedOccurences, providedDefaultOccurences, providedNotDefaultOccurences }.GetConsistency(totalOccurences)) => 
				(this.NotProvidedOccurences, this.ProvidedDefaultOccurences, this.ProvidedNotDefaultOccurences, this.NotProvidedForPublicInterfaceMembersOccurences, this.ProvidedForPublicInterfaceMembersOccurences) =
					(notProvidedOccurences, providedDefaultOccurences, providedNotDefaultOccurences, notProvidedForPublicInterfaceMembersOccurences, providedForPublicInterfaceMembersOccurences);

		public AccessibilityModifierData Update(AccessibilityModifierDataOccurence occurence, bool isFromPublicInterfaceMember)
		{
			var notProvidedForPublicInterfaceMembersOccurences = isFromPublicInterfaceMember && occurence == AccessibilityModifierDataOccurence.NotProvided ?
				this.NotProvidedForPublicInterfaceMembersOccurences + 1 : this.NotProvidedForPublicInterfaceMembersOccurences;
			var providedForPublicInterfaceMembersOccurences = isFromPublicInterfaceMember && 
				(occurence == AccessibilityModifierDataOccurence.ProvidedNotDefault || occurence == AccessibilityModifierDataOccurence.ProvidedDefault) ?
				this.ProvidedForPublicInterfaceMembersOccurences + 1 : this.ProvidedForPublicInterfaceMembersOccurences;

			return new AccessibilityModifierData(this.TotalOccurences + 1,
				occurence == AccessibilityModifierDataOccurence.NotProvided ? this.NotProvidedOccurences + 1 : this.NotProvidedOccurences,
				occurence == AccessibilityModifierDataOccurence.ProvidedDefault ? this.ProvidedDefaultOccurences + 1 : this.ProvidedDefaultOccurences,
				occurence == AccessibilityModifierDataOccurence.ProvidedNotDefault ? this.ProvidedNotDefaultOccurences + 1 : this.ProvidedNotDefaultOccurences,
				notProvidedForPublicInterfaceMembersOccurences, providedForPublicInterfaceMembersOccurences);
		}

		public string GetSetting(string name, Severity severity)
		{
			if (this.TotalOccurences > 0)
			{
				string value;

				if (this.ProvidedNotDefaultOccurences >= this.ProvidedDefaultOccurences &&
					this.ProvidedNotDefaultOccurences >= this.NotProvidedOccurences)
				{
					value = this.ProvidedNotDefaultOccurences >= (this.ProvidedDefaultOccurences + this.NotProvidedOccurences) ||
						this.ProvidedDefaultOccurences >= this.NotProvidedOccurences ?
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

				if(value == AccessibilityModifierData.ValueAlways)
				{
					if(this.NotProvidedForPublicInterfaceMembersOccurences >= this.ProvidedForPublicInterfaceMembersOccurences)
					{
						value = AccessibilityModifierData.ValueForNonInterfaceMembers;
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
				this.ProvidedNotDefaultOccurences + data.ProvidedNotDefaultOccurences,
				this.NotProvidedForPublicInterfaceMembersOccurences + data.NotProvidedForPublicInterfaceMembersOccurences,
				this.ProvidedForPublicInterfaceMembersOccurences + data.ProvidedForPublicInterfaceMembersOccurences);
		}

		public bool Equals(AccessibilityModifierData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.NotProvidedOccurences == other.NotProvidedOccurences &&
					this.ProvidedDefaultOccurences == other.ProvidedDefaultOccurences &&
					this.ProvidedNotDefaultOccurences == other.ProvidedNotDefaultOccurences &&
					this.NotProvidedForPublicInterfaceMembersOccurences == other.NotProvidedForPublicInterfaceMembersOccurences &&
					this.ProvidedForPublicInterfaceMembersOccurences == other.ProvidedForPublicInterfaceMembersOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as AccessibilityModifierData);

		public override int GetHashCode() =>
			HashCode.Combine(this.TotalOccurences, this.NotProvidedOccurences, this.ProvidedDefaultOccurences, this.ProvidedNotDefaultOccurences,
				this.NotProvidedForPublicInterfaceMembersOccurences, this.ProvidedForPublicInterfaceMembersOccurences);

		public override string ToString() =>
			$"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.NotProvidedOccurences)} = {this.NotProvidedOccurences}, {nameof(this.ProvidedDefaultOccurences)} = {this.ProvidedDefaultOccurences}, {nameof(this.ProvidedNotDefaultOccurences)} = {this.ProvidedNotDefaultOccurences}, {nameof(this.NotProvidedForPublicInterfaceMembersOccurences)} = {this.NotProvidedForPublicInterfaceMembersOccurences}, {nameof(this.ProvidedForPublicInterfaceMembersOccurences)} = {this.ProvidedForPublicInterfaceMembersOccurences}";

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

		public uint NotProvidedForPublicInterfaceMembersOccurences { get; }
		public uint NotProvidedOccurences { get; }
		public uint ProvidedForPublicInterfaceMembersOccurences { get; }
		public uint ProvidedDefaultOccurences { get; }
		public uint ProvidedNotDefaultOccurences { get; }
	}
}