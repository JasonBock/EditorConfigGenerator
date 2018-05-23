using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class TabSpaceData
		: Data<TabSpaceData>, IEquatable<TabSpaceData>
	{
		public TabSpaceData() 
			: base(default) { }

		public TabSpaceData(uint totalOccurences,
			uint tabOccurences, uint spaceOccurences)
			: base(totalOccurences)
		{
			this.TabOccurences = tabOccurences;
			this.SpaceOccurences = spaceOccurences;
		}

		public TabSpaceData Update(bool isTab) =>
			new TabSpaceData(this.TotalOccurences + 1,
				isTab ? this.TabOccurences + 1 : this.TabOccurences,
				!isTab ? this.SpaceOccurences + 1 : this.SpaceOccurences);

		public override TabSpaceData Add(TabSpaceData data)
		{
			if(data == null) { throw new ArgumentNullException(nameof(data)); }
			return new TabSpaceData(
				this.TotalOccurences + data.TotalOccurences,
				this.TabOccurences + data.TabOccurences,
				this.SpaceOccurences + data.SpaceOccurences);
		}

		public bool Equals(TabSpaceData other)
		{
			var areEqual = false;

			if (other != null)
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.TabOccurences == other.TabOccurences &&
					this.SpaceOccurences == other.SpaceOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as TabSpaceData);

		public override int GetHashCode() =>
			this.TotalOccurences.GetHashCode() ^
			(this.TabOccurences.GetHashCode() << 1) ^
			(this.SpaceOccurences.GetHashCode() << 2);

		public override string ToString() => $"{this.TotalOccurences}, {this.TabOccurences}, {this.SpaceOccurences}";

		public static bool operator ==(TabSpaceData a, TabSpaceData b)
		{
			var areEqual = false;

			if (object.ReferenceEquals(a, b))
			{
				areEqual = true;
			}

			if ((object)a != null && (object)b != null)
			{
				areEqual = a.Equals(b);
			}

			return areEqual;
		}

		public static bool operator !=(TabSpaceData a, TabSpaceData b) =>
			!(a == b);

		public uint SpaceOccurences { get; }
		public uint TabOccurences { get; }
	}
}
