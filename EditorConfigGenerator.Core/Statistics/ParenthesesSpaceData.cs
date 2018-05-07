using System;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ParenthesesSpaceData
		: Data<ParenthesesSpaceData>
	{
		public ParenthesesSpaceData()
			: base(default) { }

		public ParenthesesSpaceData(uint totalOccurences,
			uint controlFlowNoSpaceOccurences, uint controlFlowSpaceOccurences,
			uint expressionsNoSpaceOccurences, uint expressionsSpaceOccurences,
			uint typeCastsNoSpaceOccurences, uint typeCastsSpaceOccurences)
			: base(totalOccurences)
		{
			this.ControlFlowNoSpaceOccurences = controlFlowNoSpaceOccurences;
			this.ControlFlowSpaceOccurences = controlFlowSpaceOccurences;
			this.ExpressionsNoSpaceOccurences = expressionsNoSpaceOccurences;
			this.ExpressionsSpaceOccurences = expressionsSpaceOccurences;
			this.TypeCastsNoSpaceOccurences = typeCastsNoSpaceOccurences;
			this.TypeCastsSpaceOccurences = typeCastsSpaceOccurences;
		}

		public ParenthesesSpaceData UpdateControlFlow(bool hasSpaces) =>
			new ParenthesesSpaceData(this.TotalOccurences + 1,
				hasSpaces ? this.ControlFlowNoSpaceOccurences : this.ControlFlowNoSpaceOccurences + 1,
				hasSpaces ? this.ControlFlowSpaceOccurences + 1 : this.ControlFlowSpaceOccurences,
				this.ExpressionsNoSpaceOccurences, this.ExpressionsSpaceOccurences,
				this.TypeCastsNoSpaceOccurences, this.TypeCastsSpaceOccurences);

		public ParenthesesSpaceData UpdateExpression(bool hasSpaces) =>
			new ParenthesesSpaceData(this.TotalOccurences + 1,
				this.ControlFlowNoSpaceOccurences, this.ControlFlowSpaceOccurences,
				hasSpaces ? this.ExpressionsNoSpaceOccurences : this.ExpressionsNoSpaceOccurences + 1,
				hasSpaces ? this.ExpressionsSpaceOccurences + 1 : this.ExpressionsSpaceOccurences,
				this.TypeCastsNoSpaceOccurences, this.TypeCastsSpaceOccurences);

		public ParenthesesSpaceData UpdateTypeCast(bool hasSpaces) =>
			new ParenthesesSpaceData(this.TotalOccurences + 1,
				this.ControlFlowNoSpaceOccurences, this.ControlFlowSpaceOccurences,
				this.ExpressionsNoSpaceOccurences, this.ExpressionsSpaceOccurences,
				hasSpaces ? this.TypeCastsNoSpaceOccurences : this.TypeCastsNoSpaceOccurences + 1,
				hasSpaces ? this.TypeCastsSpaceOccurences + 1 : this.TypeCastsSpaceOccurences);

		public override ParenthesesSpaceData Add(ParenthesesSpaceData data)
		{
			if (data == null) { throw new ArgumentNullException(nameof(data)); }
			return new ParenthesesSpaceData(
				this.TotalOccurences + data.TotalOccurences,
				this.ControlFlowNoSpaceOccurences + data.ControlFlowNoSpaceOccurences,
				this.ControlFlowSpaceOccurences + data.ControlFlowSpaceOccurences,
				this.ExpressionsNoSpaceOccurences + data.ExpressionsNoSpaceOccurences,
				this.ExpressionsSpaceOccurences + data.ExpressionsSpaceOccurences,
				this.TypeCastsNoSpaceOccurences + data.TypeCastsNoSpaceOccurences,
				this.TypeCastsSpaceOccurences + data.TypeCastsSpaceOccurences);
		}

		public uint ControlFlowNoSpaceOccurences { get; }
		public uint ControlFlowSpaceOccurences { get; }
		public uint ExpressionsNoSpaceOccurences { get; }
		public uint ExpressionsSpaceOccurences { get; }
		public uint TypeCastsNoSpaceOccurences { get; }
		public uint TypeCastsSpaceOccurences { get; }
	}
}
