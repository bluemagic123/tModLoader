using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items.Placeable
{
	public class ExampleTorch : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded torch.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

			ItemID.Sets.Torches[Type] = true;
		}

		public override void SetDefaults() {
			Item.flame = true;
			Item.noWet = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.ExampleTorch>();
			Item.width = 10;
			Item.height = 12;
			Item.value = 50;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) { //Overrides the default sorting method of this Item.
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches; //Vanilla usually matches sorting methods with the right type of item, but sometimes, like with torches, it doesn't. Make sure to set whichever items manually if need be. 
		}

		public override void TorchVFX(out float R, out float G, out float B, out int DustType) {
			R = 1f;
			G = 1f;
			B = 1f;
			DustType = ModContent.DustType<Sparkle>();
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
