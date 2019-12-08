using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static EditorConfigGenerator.Core.Extensions.ListOfUintsExtensions;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ModifierData
		: Data<ModifierData>, IEquatable<ModifierData?>
	{
		public ModifierData()
			: base(default, default) { }

		private ModifierData(ImmutableDictionary<string, (uint weight, uint frequency)> visibilityModifiers,
			ImmutableDictionary<string, (uint weight, uint frequency)> otherModifiers)
			: this((uint)visibilityModifiers.Values.Sum(_ => _.frequency),
				(uint)otherModifiers.Values.Sum(_ => _.frequency))
		{
			this.VisibilityModifiers = visibilityModifiers;
			this.OtherModifiers = otherModifiers;
		}

		private ModifierData(uint visibilityModifiers, uint otherModifiers)
			: base(visibilityModifiers + otherModifiers, new List<uint> { visibilityModifiers, otherModifiers }.GetConsistency(visibilityModifiers + otherModifiers)) { }

		public override ModifierData Add(ModifierData data)
		{
			if (data is null) { throw new ArgumentNullException(nameof(data)); }

			var currentVisibilityModifers = this.VisibilityModifiers.ToBuilder();

			foreach(var visibilityPair in data.VisibilityModifiers)
			{
				var (weight, frequency) = currentVisibilityModifers[visibilityPair.Key];
				currentVisibilityModifers[visibilityPair.Key] = (weight + visibilityPair.Value.weight,
					frequency + visibilityPair.Value.frequency);
			}

			var currentOtherModifers = this.OtherModifiers.ToBuilder();

			foreach (var otherPair in data.OtherModifiers)
			{
				var (weight, frequency) = currentOtherModifers[otherPair.Key];
				currentOtherModifers[otherPair.Key] = (weight + otherPair.Value.weight,
					frequency + otherPair.Value.frequency);
			}

			return new ModifierData(currentVisibilityModifers.ToImmutableDictionary(),
				currentOtherModifers.ToImmutableDictionary());
		}

		public ModifierData Update(ImmutableList<string> modifiers)
		{
			if (modifiers is null) { throw new ArgumentNullException(nameof(modifiers)); }

			var currentVisibilityModifers = this.VisibilityModifiers.ToBuilder();
			var currentOtherModifers = this.OtherModifiers.ToBuilder();

			for (var i = 0; i < modifiers.Count; i++)
			{
				var modifier = modifiers[i];

				if(currentVisibilityModifers.ContainsKey(modifier))
				{
					var (weight, frequency) = currentVisibilityModifers[modifier];
					currentVisibilityModifers[modifier] = 
						(weight + ((uint)modifiers.Count - (uint)i), frequency + 1);
				}
				else if (currentOtherModifers.ContainsKey(modifier))
				{
					var (weight, frequency) = currentOtherModifers[modifier];
					currentOtherModifers[modifier] = 
						(weight + ((uint)modifiers.Count - (uint)i), frequency + 1);
				}
			}

			return new ModifierData(currentVisibilityModifers.ToImmutableDictionary(),
				currentOtherModifers.ToImmutableDictionary());
		}

		public bool Equals(ModifierData? other)
		{
			var areEqual = false;

			if (other is { })
			{
				areEqual = this.TotalOccurences == other.TotalOccurences;

				if(areEqual)
				{
					foreach(var visibilityPair in this.VisibilityModifiers)
					{
						areEqual &= visibilityPair.Value == other.VisibilityModifiers[visibilityPair.Key];

						if(!areEqual) { return areEqual; }
					}

					foreach (var otherPair in this.OtherModifiers)
					{
						areEqual &= otherPair.Value == other.OtherModifiers[otherPair.Key];

						if (!areEqual) { return areEqual; }
					}
				}
			}

			return areEqual;
		}

		public override bool Equals(object obj) => this.Equals(obj as ModifierData);

		public override int GetHashCode() => this.ToString().GetHashCode();

		public override string ToString()
		{
			var visibilityModifiers = new List<string>();
			var otherModifiers = new List<string>();

			foreach (var visibilityPair in this.VisibilityModifiers.OrderBy(_ => _.Key))
			{
				visibilityModifiers.Add($"{visibilityPair.Key} = {visibilityPair.Value}");
			}

			foreach (var otherPair in this.OtherModifiers.OrderBy(_ => _.Key))
			{
				otherModifiers.Add($"{otherPair.Key} = {otherPair.Value}");
			}

			return $"{nameof(this.TotalOccurences)} = {this.TotalOccurences}, {nameof(this.VisibilityModifiers)} = {{{string.Join(", ", visibilityModifiers)}}}, {nameof(this.OtherModifiers)} = {{{string.Join(", ", otherModifiers)}}}";
		}

		public static bool operator ==(ModifierData a, ModifierData b)
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

		public static bool operator !=(ModifierData a, ModifierData b) =>
			!(a == b);

		public ImmutableDictionary<string, (uint weight, uint frequency)> VisibilityModifiers =
			new Dictionary<string, (uint weight, uint frequency)>
			{
				{ SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.PrivateKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.ProtectedKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.InternalKeyword).ValueText, (0u, 0u) },
			}.ToImmutableDictionary();

		public ImmutableDictionary<string, (uint weight, uint frequency)> OtherModifiers =
			new Dictionary<string, (uint weight, uint frequency)>
			{
				{ SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.ExternKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.NewKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.VirtualKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.AbstractKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.SealedKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.OverrideKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.UnsafeKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.VolatileKeyword).ValueText, (0u, 0u) },
				{ SyntaxFactory.Token(SyntaxKind.AsyncKeyword).ValueText, (0u, 0u) },
			}.ToImmutableDictionary();
	}
}