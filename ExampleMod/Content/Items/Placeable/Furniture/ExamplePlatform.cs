using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExampleMod.Content.Items.Placeable.Furniture
{
	public class ExamplePlatform : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded platform.");
		}

		public override void SetDefaults() {
			item.width = 8;
			item.height = 10;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.Swing;
			item.consumable = true;
			item.createTile = TileType<Tiles.Furniture.ExamplePlatform>();
		}

		public override void AddRecipes() {
			CreateRecipe(2)
				.AddIngredient<ExampleBlock>()
				.AddTile<Tiles.Furniture.ExampleWorkbench>()
				.Register();
		}
	}
}