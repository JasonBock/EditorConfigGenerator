using System;
using System.Collections.Generic;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ParenthesesSpaceData
		: Data<ParenthesesSpaceData>, IEquatable<ParenthesesSpaceData?>
	{
		public ParenthesesSpaceData()
			: base(default, default) { }

		public ParenthesesSpaceData(uint totalOccurences,
			uint controlFlowNoSpaceOccurences, uint controlFlowSpaceOccurences,
			uint expressionsNoSpaceOccurences, uint expressionsSpaceOccurences,
			uint typeCastsNoSpaceOccurences, uint typeCastsSpaceOccurences)
			: base(totalOccurences, new List<uint> { controlFlowNoSpaceOccurences + expressionsNoSpaceOccurences + typeCastsNoSpaceOccurences,
				controlFlowSpaceOccurences + expressionsSpaceOccurences + typeCastsSpaceOccurences }.GetConsistency(totalOccurences))
		{
			if(controlFlowNoSpaceOccurences + controlFlowSpaceOccurences + expressionsNoSpaceOccurences + expressionsSpaceOccurences +
				typeCastsNoSpaceOccurences + typeCastsSpaceOccurences != totalOccurences)
			{
				throw new InvalidOccurenceValuesException(
					$"{controlFlowNoSpaceOccurences} ({nameof(controlFlowNoSpaceOccurences)}) + {controlFlowSpaceOccurences} ({nameof(controlFlowSpaceOccurences)}) + {expressionsNoSpaceOccurences} ({nameof(expressionsNoSpaceOccurences)}) + {expressionsSpaceOccurences} ({nameof(expressionsSpaceOccurences)}) + {typeCastsNoSpaceOccurences} ({nameof(typeCastsNoSpaceOccurences)}) + {typeCastsSpaceOccurences} ({nameof(typeCastsSpaceOccurences)}) != {totalOccurences} ({nameof(totalOccurences)})");
			}

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
			if (data is null) { throw new ArgumentNullException(nameof(data)); }
			return new ParenthesesSpaceData(
				this.TotalOccurences + data.TotalOccurences,
				this.ControlFlowNoSpaceOccurences + data.ControlFlowNoSpaceOccurences,
				this.ControlFlowSpaceOccurences + data.ControlFlowSpaceOccurences,
				this.ExpressionsNoSpaceOccurences + data.ExpressionsNoSpaceOccurences,
				this.ExpressionsSpaceOccurences + data.ExpressionsSpaceOccurences,
				this.TypeCastsNoSpaceOccurences + data.TypeCastsNoSpaceOccurences,
				this.TypeCastsSpaceOccurences + data.TypeCastsSpaceOccurences);
		}

		public bool Equals(ParenthesesSpaceData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences &&
					this.ControlFlowNoSpaceOccurences == other.ControlFlowNoSpaceOccurences &&
					this.ControlFlowSpaceOccurences == other.ControlFlowSpaceOccurences &&
					this.ExpressionsNoSpaceOccurences == other.ExpressionsNoSpaceOccurences &&
					this.ExpressionsSpaceOccurences == other.ExpressionsSpaceOccurences &&
					this.TypeCastsNoSpaceOccurences == other.TypeCastsNoSpaceOccurences &&
					this.TypeCastsSpaceOccurences == other.TypeCastsSpaceOccurences;
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as ParenthesesSpaceData);

		public override int GetHashCode() =>
			this.TotalOccurences.GetHashCode() ^
			(this.ControlFlowNoSpaceOccurences.GetHashCode() << 1) ^
			(this.ControlFlowSpaceOccurences.GetHashCode() << 2) ^
			(this.ExpressionsNoSpaceOccurences.GetHashCode() << 3) ^
			(this.ExpressionsSpaceOccurences.GetHashCode() << 4) ^
			(this.TypeCastsNoSpaceOccurences.GetHashCode() << 5) ^
			(this.TypeCastsSpaceOccurences.GetHashCode() << 6);

		public override string ToString() => 
			$"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.ControlFlowNoSpaceOccurences)} = {this.ControlFlowNoSpaceOccurences}, {nameof(this.ControlFlowSpaceOccurences)} = {this.ControlFlowSpaceOccurences}, {nameof(this.ExpressionsNoSpaceOccurences)} = {this.ExpressionsNoSpaceOccurences}, {nameof(this.ExpressionsSpaceOccurences)} = {this.ExpressionsSpaceOccurences}, {nameof(this.TypeCastsNoSpaceOccurences)} = {this.TypeCastsNoSpaceOccurences}, {nameof(this.TypeCastsSpaceOccurences)} = {this.TypeCastsSpaceOccurences}";

		public static bool operator ==(ParenthesesSpaceData a, ParenthesesSpaceData b)
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

		public static bool operator !=(ParenthesesSpaceData a, ParenthesesSpaceData b) =>
			!(a == b);

		public uint ControlFlowNoSpaceOccurences { get; }
		public uint ControlFlowSpaceOccurences { get; }
		public uint ExpressionsNoSpaceOccurences { get; }
		public uint ExpressionsSpaceOccurences { get; }
		public uint TypeCastsNoSpaceOccurences { get; }
		public uint TypeCastsSpaceOccurences { get; }
	}
}