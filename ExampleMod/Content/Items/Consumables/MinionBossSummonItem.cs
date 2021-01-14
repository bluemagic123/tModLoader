﻿using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using ExampleMod.Content.NPCs.MinionBoss;

namespace ExampleMod.Content.Items.Consumables
{
	//This is the item used to summon a boss, in this case the modded Minion Boss from Example Mod. For vanilla boss summons, see PlanteraItem.cs
	public class MinionBossSummonItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Minion Boss Summon Item");
			Tooltip.SetDefault("Summons Minion Boss");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
			ItemID.Sets.SortingPriorityBossSpawns[item.type] = 12; // This helps sort inventory know that this is a boss summoning item.
		}

		public override void SetDefaults() {
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.value = 100;
			item.rare = ItemRarityID.Blue;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = ItemUseStyleID.HoldUp;
			item.consumable = true;
		}

		public override bool CanUseItem(Player player) {
			//If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss
			return !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>());
		}

		public override bool UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				//If the player using the item is the client
				//(explicitely excluded serverside here)
				SoundEngine.PlaySound(SoundID.Roar, player.position, 0);

				int type = ModContent.NPCType<MinionBossBody>();

				if (Main.netMode != NetmodeID.MultiplayerClient) {
					//If the player is not in multiplayer, spawn directly
					NPC.SpawnOnPlayer(player.whoAmI, type);
				}
				else {
					//If the player is in multiplayer, request a spawn
					//This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
					NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
				}
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
}