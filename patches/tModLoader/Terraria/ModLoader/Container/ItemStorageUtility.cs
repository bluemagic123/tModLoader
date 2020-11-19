﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

#nullable enable
namespace Terraria.ModLoader.Container
{
	public static class ItemStorageUtility
	{
		public static bool Contains(this ItemStorage storage, int type) => storage.Any(item => !item.IsAir && item.type == type);

		public static bool Contains(this ItemStorage storage, Item item) => storage.Any(item.IsTheSameAs);

		/// <summary>
		/// Gets the coin value for a storage.
		/// </summary>
		public static long CountCoins(this ItemStorage storage) {
			long num = 0L;
			foreach (Item item in storage) {
				if (item.IsAir) continue;

				num += item.type switch {
					ItemID.CopperCoin => item.stack,
					ItemID.SilverCoin => item.stack * 100,
					ItemID.GoldCoin => item.stack * 10000,
					ItemID.PlatinumCoin => item.stack * 1000000,
					_ => 0
				};
			}

			return num;
		}

		/// <summary>
		/// Drops items from the storage into the rectangle specified.
		/// </summary>
		public static void DropItems(this ItemStorage storage, object? sender, Rectangle hitbox) {
			for (int i = 0; i < storage.Count; i++) {
				Item item = storage[i];
				if (!item.IsAir) {
					Item.NewItem(hitbox, item.type, item.stack, prefixGiven: item.prefix);
					storage.RemoveItem(i, sender);
				}
			}
		}

		/// <summary>
		/// Quick stacks player's items into the storage.
		/// </summary>
		public static void QuickStack(this Player player, ItemStorage storage) {
			for (int i = 49; i >= 10; i--) {
				Item inventory = player.inventory[i];

				if (!inventory.IsAir && storage.Contains(inventory.type)) 
					storage.InsertItem(ref inventory, player);
			}

			SoundEngine.PlaySound(SoundID.Grab);
		}

		/// <summary>
		/// Loots storage's items into a player's inventory
		/// </summary>
		public static void LootAll(this Player player, ItemStorage storage) {
			for (int i = 0; i < storage.Count; i++) {
				Item item = storage[i];
				if (!item.IsAir) {
					item.position = player.Center;
					item.noGrabDelay = 0;

					foreach (var split in item.Split()) {
						player.GetItem(player.whoAmI, split, GetItemSettings.LootAllSettings);
					}

					storage.RemoveItem(i, player, out _);
				}
			}
		}

		/// <summary>
		/// Loots storage's items into the player's inventory.
		/// </summary>
		public static void Loot(this Player player, ItemStorage storage, int slot) {
			Item item = storage[slot];
			if (!item.IsAir) {
				Item n = new Item(item.type);

				int count = Math.Min(item.stack, item.maxStack);
				n.stack = count;
				n.position = player.Center;
				n.noGrabDelay = 0;

				player.GetItem(player.whoAmI, n, GetItemSettings.LootAllSettings);

				storage.ModifyStackSize(slot, -count, player);
			}
		}

		/// <summary>
		/// Deposits a player's items into storage.
		/// </summary>
		public static void DepositAll(this Player player, ItemStorage storage) {
			for (int i = 49; i >= 10; i--) {
				Item item = player.inventory[i];
				if (item.IsAir || item.favorited) continue;
				storage.InsertItem(ref item, player);
			}

			SoundEngine.PlaySound(SoundID.Grab);
		}

		/// <summary>
		/// Combines several stacks of items into one stack, disregarding max stack.
		/// </summary>
		public static Item Combine(IEnumerable<Item> items) {
			Item ret = new Item();

			foreach (Item item in items) {
				if (ret.IsAir && !item.IsAir) {
					ret = item.Clone();
					ret.stack = 0;
				}

				if (ret.type == item.type) ret.stack += item.stack;
			}

			return ret;
		}

		/// <summary>
		/// Splits a stack of items into separate stacks that respect max stack.
		/// </summary>
		public static IEnumerable<Item> Split(this Item item) {
			while (item.stack > 0) {
				Item clone = item.Clone();
				int count = Math.Min(item.stack, item.maxStack);
				clone.stack = count;
				yield return clone;

				item.stack -= count;
				if (item.stack <= 0) {
					item.TurnToAir();
					yield break;
				}
			}
		}
	}
}