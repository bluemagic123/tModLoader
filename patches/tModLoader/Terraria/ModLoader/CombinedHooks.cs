﻿using System;

namespace Terraria.ModLoader
{
	public static class CombinedHooks
	{
		public static void ModifyWeaponDamage(Player player, Item item, ref Modifier damage, ref float flat) {
			ItemLoader.ModifyWeaponDamage(item, player, ref damage, ref flat);
			PlayerHooks.ModifyWeaponDamage(player, item, ref damage, ref flat);
		}

		public static void ModifyManaCost(Player player, Item item, ref float reduce, ref float mult) {
			ItemLoader.ModifyManaCost(item, player, ref reduce, ref mult);
			PlayerHooks.ModifyManaCost(player, item, ref reduce, ref mult);
		}

		public static void OnConsumeMana(Player player, Item item, int manaConsumed) {
			ItemLoader.OnConsumeMana(item, player, manaConsumed);
			PlayerHooks.OnConsumeMana(player, item, manaConsumed);
		}

		public static void OnMissingMana(Player player, Item item, int neededMana) {
			ItemLoader.OnMissingMana(item, player, neededMana);
			PlayerHooks.OnMissingMana(player, item, neededMana);
		}

		//TODO: Fix various inconsistencies with calls of UseItem, and then make this and its inner methods use short-circuiting.
		public static bool CanUseItem(Player player, Item item) {
			return PlayerHooks.CanUseItem(player, item) & ItemLoader.CanUseItem(item, player);
		}

		public static bool? CanPlayerHitNPCWithItem(Player player, Item item, NPC npc) {
			bool? result = null;

			bool ModifyResult(bool? nbool) {
				if (nbool.HasValue) {
					result = nbool.Value;
				}

				return result != false;
			}

			if (!ModifyResult(PlayerHooks.CanHitNPC(player, item, npc))) {
				return false;
			}

			if (!ModifyResult(ItemLoader.CanHitNPC(item, player, npc))) {
				return false;
			}

			if (!ModifyResult(NPCLoader.CanBeHitByItem(npc, player, item))) {
				return false;
			}

			return result;
		}
	}
}
