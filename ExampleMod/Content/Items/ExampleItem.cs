using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleItem : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded item."); //The (English) text shown below your weapon's name
		}

		public override void SetDefaults() {
			item.width = 20; //The item texture's width
			item.height = 20; //The item texture's height

			item.maxStack = 999; //The item's max stack value
			item.value = Item.buyPrice(silver: 1); //The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
			item.rare = ItemRarityID.Blue; // The rarity of the weapon.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.DirtBlock, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
