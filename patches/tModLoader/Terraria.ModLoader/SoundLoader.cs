using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace Terraria.ModLoader
{
	public static class SoundLoader
	{
		private static readonly IDictionary<SoundType, int> nextSound = new Dictionary<SoundType, int>();
		internal static readonly IDictionary<SoundType, IDictionary<string, int>> sounds = new Dictionary<SoundType, IDictionary<string, int>>();
		internal static readonly IDictionary<SoundType, IDictionary<int, ModSound>> modSounds = new Dictionary<SoundType, IDictionary<int, ModSound>>();
		internal static SoundEffect[] customSounds = new SoundEffect[0];
		internal static SoundEffectInstance[] customSoundInstances = new SoundEffectInstance[0];
		public const int customSoundType = 50;

		static SoundLoader()
		{
			foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
			{
				nextSound[type] = GetNumVanilla(type);
				sounds[type] = new Dictionary<string, int>();
				modSounds[type] = new Dictionary<int, ModSound>();
			}
		}

		internal static int ReserveSoundID(SoundType type)
		{
			int reserveID = nextSound[type];
			nextSound[type]++;
			return reserveID;
		}

		public static int GetSoundSlot(SoundType type, string sound)
		{
			if (sounds[type].ContainsKey(sound))
			{
				return sounds[type][sound];
			}
			else
			{
				return 0;
			}
		}

		internal static void ResizeAndFillArrays()
		{
			customSounds = new SoundEffect[nextSound[SoundType.Custom]];
			customSoundInstances = new SoundEffectInstance[nextSound[SoundType.Custom]];
			Array.Resize(ref Main.soundItem, nextSound[SoundType.Item]);
			Array.Resize(ref Main.soundInstanceItem, nextSound[SoundType.Item]);
			Array.Resize(ref Main.soundNPCHit, nextSound[SoundType.NPCHit]);
			Array.Resize(ref Main.soundInstanceNPCHit, nextSound[SoundType.NPCHit]);
			Array.Resize(ref Main.soundNPCKilled, nextSound[SoundType.NPCKilled]);
			Array.Resize(ref Main.soundInstanceNPCKilled, nextSound[SoundType.NPCKilled]);
			foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
			{
				foreach (string sound in sounds[type].Keys)
				{
					int slot = GetSoundSlot(type, sound);
					GetSoundArray(type)[slot] = ModLoader.GetSound(sound);
					GetSoundInstanceArray(type)[slot] = GetSoundArray(type)[slot].CreateInstance();
				}
			}
		}

		internal static void Unload()
		{
			foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
			{
				nextSound[type] = GetNumVanilla(type);
				sounds[type].Clear();
				modSounds[type].Clear();
			}
		}
		//in Terraria.Main.PlaySound before checking type to play sound add
		//  if (SoundLoader.PlayModSound(type, num, num2, num3)) { return; }
		internal static bool PlayModSound(int type, int style, float volume, float pan)
		{
			SoundType soundType;
			switch (type)
			{
				case 2:
					soundType = SoundType.Item;
					break;
				case 3:
					soundType = SoundType.NPCHit;
					break;
				case 4:
					soundType = SoundType.NPCKilled;
					break;
				default:
					return false;
			}
			if (!modSounds[soundType].ContainsKey(style))
			{
				return false;
			}
			modSounds[soundType][style].PlaySound(ref GetSoundInstanceArray(soundType)[style], volume, pan, soundType);
			return true;
		}

		internal static int GetNumVanilla(SoundType type)
		{
			switch (type)
			{
				case SoundType.Custom:
					return 0;
				case SoundType.Item:
					return Main.maxItemSounds + 1;
				case SoundType.NPCHit:
					return Main.maxNPCHitSounds + 1;
				case SoundType.NPCKilled:
					return Main.maxNPCKilledSounds + 1;
			}
			return 0;
		}

		internal static SoundEffect[] GetSoundArray(SoundType type)
		{
			switch (type)
			{
				case SoundType.Custom:
					return customSounds;
				case SoundType.Item:
					return Main.soundItem;
				case SoundType.NPCHit:
					return Main.soundNPCHit;
				case SoundType.NPCKilled:
					return Main.soundNPCKilled;
			}
			return null;
		}

		internal static SoundEffectInstance[] GetSoundInstanceArray(SoundType type)
		{
			switch (type)
			{
				case SoundType.Custom:
					return customSoundInstances;
				case SoundType.Item:
					return Main.soundInstanceItem;
				case SoundType.NPCHit:
					return Main.soundInstanceNPCHit;
				case SoundType.NPCKilled:
					return Main.soundInstanceNPCKilled;
			}
			return null;
		}
	}
}