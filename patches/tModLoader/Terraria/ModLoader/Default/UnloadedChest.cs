using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Terraria.ModLoader.Default
{
	[Autoload(false)] // need multiple versions, all subclassed
	public class UnloadedChest : UnloadedTile {

		public override string Texture => "ModLoader/UnloadedChest";

		public override void SetDefaults() {
			//common
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;

			TileID.Sets.BasicChest[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1); // Disables hammering
			TileObjectData.addTile(Type);
				
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileOreFinderPriority[Type] = 500;

			//TileID.Sets.HasOutlines[Type] = isChest;

			AdjTiles = new int[] { TileID.Containers };

			ContainerName.SetDefault("UnloadedChest");

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Unloaded Chest");
			AddMapEntry(new Color(200, 200, 200), name, MapChestName);

			name = CreateMapEntryName(Name + "_Locked");
			name.SetDefault("Locked Unloaded Chest");
			AddMapEntry(new Color(0, 141, 63), name, MapChestName);
		}

		public static string MapChestName(string name, int i, int j) {
			int left = i;
			int top = j;
			Tile tile = Main.tile[i, j];

			if (tile.frameX % 36 != 0) {
				left--;
			}

			if (tile.frameY != 0) {
				top--;
			}

			int chest = Chest.FindChest(left, top);
			if (chest < 0) {
				return Language.GetTextValue("LegacyChestType.0");
			}

			if (Main.chest[chest].name == "") {
				return name;
			}

			return name + ": " + Main.chest[chest].name;
		}
	}
}
