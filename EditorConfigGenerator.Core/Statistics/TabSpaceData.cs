using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class TabSpaceData
		: Data<TabSpaceData>
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

		public uint SpaceOccurences { get; }
		public uint TabOccurences { get; }
	}
}
