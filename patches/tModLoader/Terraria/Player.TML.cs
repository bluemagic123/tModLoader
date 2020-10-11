﻿using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Terraria
{
	public partial class Player
	{
		internal IList<string> usedMods;
		internal ModPlayer[] modPlayers = new ModPlayer[0];

		// Get

		/// <summary> Gets the instance of the specified ModPlayer type. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public T GetModPlayer<T>() where T : ModPlayer
			=> GetModPlayer(ModContent.GetInstance<T>());

		/// <summary> Gets the local instance of the type of the specified ModPlayer instance. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		/// <exception cref="NullReferenceException"/>
		public T GetModPlayer<T>(T baseInstance) where T : ModPlayer
			=> modPlayers[baseInstance.index] as T ?? throw new KeyNotFoundException($"Instance of '{typeof(T).Name}' does not exist on the current player.");

		/*
		// TryGet

		/// <summary> Gets the instance of the specified ModPlayer type. </summary>
		public bool TryGetModPlayer<T>(out T result) where T : ModPlayer
			=> TryGetModPlayer(ModContent.GetInstance<T>(), out result);

		/// <summary> Safely attempts to get the local instance of the type of the specified ModPlayer instance. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public bool TryGetModPlayer<T>(T baseInstance, out T result) where T : ModPlayer {
			if (baseInstance == null || baseInstance.index < 0 || baseInstance.index >= modPlayers.Length) {
				result = default;

				return false;
			}

			result = modPlayers[baseInstance.index] as T;

			return result != null;
		}
		*/

		// Damage Classes

		private DamageClassData[] damageData = new DamageClassData[0];

		internal void ResetDamageClassDictionaries() {
			damageData = new DamageClassData[DamageClassLoader.DamageClassCount];

			for (int i = 0; i < damageData.Length; i++) {
				damageData[i] = new DamageClassData(4, Modifier.One); // Default values from vanilla - 4 crit, 0 add, 1x mult.
			}
		}

		/// <summary>
		/// Edits the crit for the given damage type on this player.
		/// </summary>
		/// <param name="changeAmount">The amount that should be added to the crit. Can be negative too.</param>
		public void AddCrit<T>(int changeAmount) where T : DamageClass => AddCrit(ModContent.GetInstance<T>(), changeAmount);

		/// <summary>
		/// Edits the crit for the given damage type on this player.
		/// </summary>
		/// <param name="changeAmount">The amount that should be added to the crit. Can be negative too.</param>
		public void AddCrit(DamageClass damageClass, int changeAmount) {
			if (!(damageClass is Summon)) {
				damageData[damageClass.index].crit += changeAmount;
			}
		}

		/// <summary>
		/// Gets the crit stat for this damage type on this player.
		/// </summary>
		public int GetCrit<T>() where T : DamageClass => GetCrit(ModContent.GetInstance<T>());

		/// <summary>
		/// Gets the damage stat for this damage type on this player.
		/// </summary>
		public ref Modifier GetDamage<T>() where T : DamageClass => ref GetDamage(ModContent.GetInstance<T>());

		/// <summary>
		/// Gets the crit stat for this damage type on this player.
		/// </summary>
		public int GetCrit(DamageClass damageClass) => damageClass is Summon ? 0 : damageData[damageClass.index].crit; // Special case, summoner class cannot have crits.

		/// <summary>
		/// Gets the reference to the damage stat for this damage type on this player. Since this returns a reference, you can freely modify this method's return value with operators.
		/// </summary>
		public ref Modifier GetDamage(DamageClass damageClass) => ref damageData[damageClass.index].damage;
	}
}
