using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class BossItem : ExampleItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Boss Item");
			Tooltip.SetDefault("Used to craft boss items");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}