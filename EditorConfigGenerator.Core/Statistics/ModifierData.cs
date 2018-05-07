using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Statistics
{
	public sealed class ModifierData
		: Data<ModifierData>
	{
		public ModifierData()
			: base(default) { }

		private ModifierData(ImmutableDictionary<string, (uint weight, uint frequency)> visibilityModifiers,
			ImmutableDictionary<string, (uint weight, uint frequency)> otherModifiers)
			: base((uint)visibilityModifiers.Values.Sum(_ => _.frequency) +
				(uint)otherModifiers.Values.Sum(_ => _.frequency))
		{
			this.VisibilityModifiers = visibilityModifiers;
			this.OtherModifiers = otherModifiers;
		}

		public override ModifierData Add(ModifierData data)
		{
			if (data == null) { throw new ArgumentNullException(nameof(data)); }

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
			if (modifiers == null) { throw new ArgumentNullException(nameof(modifiers)); }

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

		public ImmutableDictionary<string, (uint weight, uint frequency)> VisibilityModifiers =
			new Dictionary<string, (uint weight, uint frequency)>
			{
				{ "public", (0u, 0u) },
				{ "private", (0u, 0u) },
				{ "protected", (0u, 0u) },
				{ "internal", (0u, 0u) },
			}.ToImmutableDictionary();

		public ImmutableDictionary<string, (uint weight, uint frequency)> OtherModifiers =
			new Dictionary<string, (uint weight, uint frequency)>
			{
				{ "static", (0u, 0u) },
				{ "extern", (0u, 0u) },
				{ "new", (0u, 0u) },
				{ "virtual", (0u, 0u) },
				{ "abstract", (0u, 0u) },
				{ "sealed", (0u, 0u) },
				{ "override", (0u, 0u) },
				{ "readonly", (0u, 0u) },
				{ "unsafe", (0u, 0u) },
				{ "volatile", (0u, 0u) },
				{ "async", (0u, 0u) },
			}.ToImmutableDictionary();
	}
}