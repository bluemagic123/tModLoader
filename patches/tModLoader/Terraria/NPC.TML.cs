﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace Terraria
{
	public partial class NPC
	{
		public ModNPC ModNPC { get; internal set; }

		internal GlobalNPC[] globalNPCs = new GlobalNPC[0];

		/// <summary>
		/// Assign a special boss bar, vanilla or modded. Not used by vanilla.
		/// <para>To assign a modded boss bar, use NPC.BossBar = ModContent.GetInstance<ExampleBossBar>(); where ExampleBossBar is a ModBossBar</para>
		/// <para>To assign a vanilla boss bar for whatever reason, fetch it first through the NPC type using Main.BigBossProgressBar.TryGetSpecialVanillaBossBar</para>
		/// </summary>
		public IBigProgressBar BossBar { get; set; }

		// Get

		/// <summary> Gets the instance of the specified GlobalNPC type. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public T GetGlobalNPC<T>() where T : GlobalNPC
			=> GetGlobalNPC(ModContent.GetInstance<T>());

		/// <summary> Gets the local instance of the type of the specified GlobalNPC instance. This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		/// <exception cref="NullReferenceException"/>
		public T GetGlobalNPC<T>(T baseInstance) where T : GlobalNPC
			=> baseInstance.Instance(this) as T ?? throw new KeyNotFoundException($"Instance of '{typeof(T).Name}' does not exist on the current npc.");

		/*
		// TryGet

		/// <summary> Gets the instance of the specified GlobalNPC type. </summary>
		public bool TryGetGlobalNPC<T>(out T result) where T : GlobalNPC
			=> TryGetGlobalNPC(ModContent.GetInstance<T>(), out result);

		/// <summary> Safely attempts to get the local instance of the type of the specified GlobalNPC instance. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public bool TryGetGlobalNPC<T>(T baseInstance, out T result) where T : GlobalNPC {
			if (baseInstance == null || baseInstance.index < 0 || baseInstance.index >= globalNPCs.Length) {
				result = default;

				return false;
			}

			result = baseInstance.Instance(this) as T;

			return result != null;
		}
		*/
	}
}
