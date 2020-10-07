﻿using ExampleMod.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Content.Pets.ExamplePet
{
	public class ExamplePetItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Paper Airplane");
			Tooltip.SetDefault("Summons a Paper Airplane to follow aimlessly behind you");
		}

		public override void SetDefaults() {
			item.CloneDefaults(ItemID.ZephyrFish); // Copy the Defaults of the Zephyr Fish item.

			item.shoot = ProjectileType<ExamplePetProjectile>(); // "Shoot" your pet projectile.
			item.buffType = BuffType<ExamplePetBuff>(); // Apply buff upon usage of the item.
		}

		public override void UseStyle(Player player) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(item.buffType, 3600);
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}
