﻿using Terraria.Localization;

namespace Terraria.ModLoader
{
	public abstract class DamageClass : ModType
	{
		public static Melee Melee { get; private set; } = new Melee();
		public static Ranged Ranged { get; private set; } = new Ranged();
		public static Magic Magic { get; private set; } = new Magic();
		public static Summon Summon { get; private set; } = new Summon();

		internal int index;

		/// <summary>
		/// This is the name that will show up when an item tooltip displays 'X [ClassName]'. This should include the 'damage' part.
		/// </summary>
		public ModTranslation ClassName { get; internal set; }

		protected override void Register() {
			index = DamageClassLoader.Add(this);

			ClassName = Mod.GetOrCreateTranslation($"Mods.{Mod.Name}.BuffName.{Name}");

			ModTypeLookup<DamageClass>.Register(this);
		}
	}
}
