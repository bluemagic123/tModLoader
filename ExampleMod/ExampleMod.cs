using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ExampleMod.NPCs.PuritySpirit;
using ExampleMod.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ExampleMod.UI;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using ExampleMod.Fonts;

namespace ExampleMod
{
	public class ExampleMod : Mod
	{
		public const string captiveElementHead = "ExampleMod/NPCs/Abomination/CaptiveElement_Head_Boss_";
		public const string captiveElement2Head = "ExampleMod/NPCs/Abomination/CaptiveElement2_Head_Boss_";
		public static SpriteFont exampleFont;
        	public static SpriteFontX exampleCustomFont;
		private UserInterface exampleUserInterface;
		internal ExampleUI exampleUI;
		public static ModHotKey RandomBuffHotKey;
		public static int FaceCustomCurrencyID;

		public ExampleMod()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true,
				AutoloadBackgrounds = true
			};
		}

		public override void Load()
		{
			for (int k = 1; k <= 4; k++)
			{
				AddBossHeadTexture(captiveElementHead + k);
				AddBossHeadTexture(captiveElement2Head + k);
			}
			RandomBuffHotKey = RegisterHotKey("Random Buff", "P");
			FaceCustomCurrencyID = CustomCurrencyManager.RegisterCurrency(new ExampleCustomCurrency(ItemType<Items.Face>(), 999L));
			if (!Main.dedServ)
			{
				AddEquipTexture(null, EquipType.Legs, "ExampleRobe_Legs", "ExampleMod/Items/Armor/ExampleRobe_Legs");
				//Main.music[MusicID.Dungeon].ModMusic = GetSound("Sounds/Music/ExampleMusic").CreateInstance();
				AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/DriveMusic"), ItemType("ExampleMusicBox"), TileType("ExampleMusicBox"));
				Main.instance.LoadTiles(TileID.Loom);
				Main.tileTexture[TileID.Loom] = GetTexture("Tiles/AnimatedLoom");
				// What if....Replace a vanilla item texture and equip texture.
				//Main.itemTexture[ItemID.CopperHelmet] = GetTexture("Resprite/CopperHelmet_Item");
				//Item copperHelmet = new Item();
				//copperHelmet.SetDefaults(ItemID.CopperHelmet);
				//Main.armorHeadLoaded[copperHelmet.headSlot] = true;
				//Main.armorHeadTexture[copperHelmet.headSlot] = GetTexture("Resprite/CopperHelmet_Head");
				Filters.Scene["ExampleMod:PuritySpirit"] = new Filter(new PuritySpiritScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.9f, 0.4f).UseOpacity(0.7f), EffectPriority.VeryHigh);
				SkyManager.Instance["ExampleMod:PuritySpirit"] = new PuritySpiritSky();
				Filters.Scene["ExampleMod:MonolithVoid"] = new Filter(new ScreenShaderData("FilterMoonLord"), EffectPriority.Medium);
				SkyManager.Instance["ExampleMod:MonolithVoid"] = new VoidSky();
				exampleFont = GetFont("Fonts/ExampleFont");
				exampleUI = new ExampleUI();
				exampleUI.Activate();
				exampleUserInterface = new UserInterface();
				exampleUserInterface.SetState(exampleUI);
				//Example loading process
                		exampleCustomFont = new SpriteFontX(new System.Drawing.Font("Arial", 12), Main.graphics, 
                    			System.Drawing.Text.TextRenderingHint.ClearTypeGridFit);
			}
		}

		public override void Unload()
		{
			if (!Main.dedServ)
			{
				Main.music[MusicID.Dungeon].ModMusic = null;
				Main.tileFrame[TileID.Loom] = 0;
				Main.tileSetsLoaded[TileID.Loom] = false;
			}
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Lang.misc[37] + " " + GetItem("ExampleItem").item.name, new int[]
			{
				ItemType("ExampleItem"),
				ItemType("EquipMaterial"),
				ItemType("BossItem")
			});
			RecipeGroup.RegisterGroup("ExampleMod:ExampleItem", group);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.Wood, 999);
			recipe.AddRecipe();
			recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.Silk, 999);
			recipe.AddRecipe();
			recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.IronOre, 999);
			recipe.AddRecipe();
			recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.GravitationPotion, 20);
			recipe.AddRecipe();
			recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.GoldChest);
			recipe.AddRecipe();
			recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(ItemID.MusicBoxDungeon);
			recipe.AddRecipe();
			RecipeHelper.AddBossRecipes(this);
			RecipeHelper.TestRecipeEditor(this);
		}

		public override void UpdateMusic(ref int music)
		{
			if (Main.myPlayer != -1 && !Main.gameMenu)
			{
				if (Main.LocalPlayer.active && Main.LocalPlayer.FindBuffIndex(this.BuffType("CarMount")) != -1)
				{
					music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/DriveMusic");
				}
				if (Main.LocalPlayer.active && Main.LocalPlayer.GetModPlayer<ExamplePlayer>(this).ZoneExample)
				{
					music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/DriveMusic");
				}
			}
		}

		const int ShakeLength = 5;
		int ShakeCount = 0;
		float previousRotation = 0;
		float targetRotation = 0;
		float previousOffsetX = 0;
		float previousOffsetY = 0;
		float targetOffsetX = 0;
		float targetOffsetY = 0;

		public override Matrix ModifyTransformMatrix(Matrix Transform)
		{
			if (!Main.gameMenu)
			{
				ExampleWorld world = GetModWorld<ExampleWorld>();
				if (world.VolcanoTremorTime > 0)
				{
					if (world.VolcanoTremorTime % ShakeLength == 0)
					{
						ShakeCount = 0;
						previousRotation = targetRotation;
						previousOffsetX = targetOffsetX;
						previousOffsetY = targetOffsetY;
						targetRotation = (Main.rand.NextFloat() - .5f) * MathHelper.ToRadians(15);
						targetOffsetX = Main.rand.Next(60) - 30;
						targetOffsetY = Main.rand.Next(40) - 20;
						if (world.VolcanoTremorTime == ShakeLength)
						{
							targetRotation = 0;
							targetOffsetX = 0;
							targetOffsetY = 0;
						}
					}
					float transX = Main.screenWidth / 2;
					float transY = Main.screenHeight / 2;

					float lerp = (float)(ShakeCount) / ShakeLength;
					float rotation = MathHelper.Lerp(previousRotation, targetRotation, lerp);
					float offsetX = MathHelper.Lerp(previousOffsetX, targetOffsetX, lerp);
					float offsetY = MathHelper.Lerp(previousOffsetY, targetOffsetY, lerp);

					world.VolcanoTremorTime--;
					ShakeCount++;


					return Transform
						* Matrix.CreateTranslation(-transX, -transY, 0f)
						* Matrix.CreateRotationZ(rotation)
						* Matrix.CreateTranslation(transX, transY, 0f)
						* Matrix.CreateTranslation(offsetX, offsetY, 0f);
					//Matrix.CreateFromAxisAngle(new Vector3(Main.screenWidth / 2, Main.screenHeight / 2, 0f), .2f);
					//Matrix.CreateRotationZ(MathHelper.ToRadians(30));
				}
			}
			return Transform;
		}
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            /* Example SpriteFontX drawing process
             *  
             *  spriteBatch.DrawString(exampleCustomFont, "Test String", new Vector2(500, 500), Color.Yellow);
             * 
             */

        }
        public override void ChatInput(string text, ref bool broadcast)
		{
			if (text[0] != '/')
			{
				return;
			}
			text = text.Substring(1);
			int index = text.IndexOf(' ');
			string command;
			string[] args;
			if (index < 0)
			{
				command = text;
				args = new string[0];
			}
			else
			{
				command = text.Substring(0, index);
				args = text.Substring(index + 1).Split(' ');
			}
			if (command == "npc")
			{
				NPCCommand(args);
				broadcast = false;
			}
			else if (command == "npcType")
			{
				NPCTypeCommand(args);
				broadcast = false;
			}
			else if (command == "addTime")
			{
				AddTimeCommand(args);
				broadcast = false;
			}
			else if (command == "item")
			{
				ItemCommand(args);
				broadcast = false;
			}
			else if (command == "score")
			{
				ScoreCommand(args);
				broadcast = false;
			}
			else if (command == "sound")
			{
				SoundCommand(args);
				broadcast = false;
			}
			else if (command == "coin")
			{
				ExampleUI.visible = true;
				broadcast = false;
			}
		}

		private void NPCCommand(string[] args)
		{
			int type;
			if (args.Length == 0 || !Int32.TryParse(args[0], out type))
			{
				Main.NewText("Usage: /npc type [x] [y] [number]");
				Main.NewText("x and y may be preceded by ~ to use position relative to player");
				return;
			}
			try
			{
				Player player = Main.LocalPlayer;
				int x;
				int y;
				int num = 1;
				if (args.Length > 2)
				{
					bool relativeX = false;
					bool relativeY = false;
					if (args[1][0] == '~')
					{
						relativeX = true;
						args[1] = args[1].Substring(1);
					}
					if (args[2][0] == '~')
					{
						relativeY = true;
						args[2] = args[2].Substring(1);
					}
					if (!Int32.TryParse(args[1], out x))
					{
						x = 0;
						relativeX = true;
					}
					if (!Int32.TryParse(args[2], out y))
					{
						y = 0;
						relativeY = true;
					}
					if (relativeX)
					{
						x += (int)player.Bottom.X;
					}
					if (relativeY)
					{
						y += (int)player.Bottom.Y;
					}
					if (args.Length > 3)
					{
						if (!Int32.TryParse(args[3], out num))
						{
							num = 1;
						}
					}
				}
				else
				{
					x = (int)player.Bottom.X;
					y = (int)player.Bottom.Y;
				}
				for (int k = 0; k < num; k++)
				{
					if (Main.netMode == 0)
					{
						NPC.NewNPC(x, y, type);
					}
					else if (Main.netMode == 1)
					{
						var netMessage = GetPacket();
						netMessage.Write((byte)ExampleModMessageType.SpawnNPC);
						netMessage.Write(x);
						netMessage.Write(y);
						netMessage.Write(type);
						netMessage.Send();
					}
				}
			}
			catch
			{
				Main.NewText("Usage: /npc type [x] [y] [number]");
				Main.NewText("x and y may be preceded by ~ to use position relative to player");
			}
		}

		private void NPCTypeCommand(string[] args)
		{
			if (args.Length < 2)
			{
				Main.NewText("Usage: /npcType modName npcName");
				return;
			}
			Mod mod = ModLoader.GetMod(args[0]);
			int type = mod == null ? 0 : mod.NPCType(args[1]);
			Main.NewText(type.ToString(), 255, 255, 0);
		}

		private void AddTimeCommand(string[] args)
		{
			int amount;
			if (args.Length == 0 || !Int32.TryParse(args[0], out amount))
			{
				Main.NewText("Usage: /addTime numTicks");
				return;
			}
			Main.time += amount;
		}

		private void ItemCommand(string[] args)
		{
			if (args.Length == 0)
			{
				Main.NewText("Usage: /item [type|name] [stack]");
				return;
			}
			try
			{
				Player player = Main.LocalPlayer;
				int type;
				if (!Int32.TryParse(args[0], out type))
				{
					args[0] = args[0].Replace("_", " ");
					for (int k = 0; k < Main.itemName.Length; k++)
					{
						if (args[0] == Main.itemName[k])
						{
							type = k;
							break;
						}
					}
				}
				int stack;
				if (args.Length < 2 || !Int32.TryParse(args[1], out stack))
				{
					stack = 1;
				}
				player.QuickSpawnItem(type, stack);
			}
			catch
			{
				Main.NewText("Usage: /item [type|name] [stack]");
			}
		}

		private void ScoreCommand(string[] args)
		{
			if (args.Length < 2 || (args[1] != "add" && args[1] != "set" && args[1] != "reset" && args[1] != "get"))
			{
				Main.NewText("Usage: /score playerName <get|add|set|reset>");
				return;
			}
			int player;
			for (player = 0; player < 255; player++)
			{
				if (Main.player[player].active && Main.player[player].name == args[0])
				{
					break;
				}
			}
			if (player == 255)
			{
				Main.NewText("Could not find player: " + args[0]);
				return;
			}
			ExamplePlayer modPlayer = Main.player[player].GetModPlayer<ExamplePlayer>(this);
			if (args[1] == "get")
			{
				Main.NewText(args[0] + "'s score is " + modPlayer.score);
				return;
			}
			if (args[1] == "reset")
			{
				modPlayer.score = 0;
				Main.NewText(args[0] + "'s score is now " + modPlayer.score);
				return;
			}
			if (args.Length < 3)
			{
				Main.NewText("Usage: /score playerName <add|set> amount");
				return;
			}
			int arg;
			if (!Int32.TryParse(args[2], out arg))
			{
				Main.NewText(args[2] + " is not an integer");
				return;
			}
			if (args[1] == "add")
			{
				modPlayer.score += arg;
			}
			else
			{
				modPlayer.score = arg;
			}
			Main.NewText(args[0] + "'s score is now " + modPlayer.score);
		}

		private void SoundCommand(string[] args)
		{
			if (args.Length < 2)
			{
				Main.NewText("Usage: /sound type style");
				return;
			}
			int type;
			if (!Int32.TryParse(args[0], out type))
			{
				Main.NewText(args[0] + " is not an integer");
				return;
			}
			int style;
			if (!Int32.TryParse(args[1], out style))
			{
				Main.NewText(args[1] + " is not an integer");
				return;
			}
			Main.PlaySound(type, -1, -1, style);
		}

		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new MethodSequenceListItem(
					"ExampleMod: Coins Per Minute",
					delegate
					{
						if (ExampleUI.visible)
						{
							exampleUserInterface.Update(Main._drawInterfaceGameTime);
							exampleUI.Draw(Main.spriteBatch);
						}
						return true;
					},
					null)
				);
			}
		}

		//spawning helper methods imported from my tAPI mod
		public static bool NoInvasion(NPCSpawnInfo spawnInfo)
		{
			return !spawnInfo.invasion && ((!Main.pumpkinMoon && !Main.snowMoon) || spawnInfo.spawnTileY > Main.worldSurface || Main.dayTime) && (!Main.eclipse || spawnInfo.spawnTileY > Main.worldSurface || !Main.dayTime);
		}

		public static bool NoBiome(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.player;
			return !player.ZoneJungle && !player.ZoneDungeon && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneSnow && !player.ZoneUndergroundDesert;
		}

		public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo)
		{
			return !spawnInfo.sky && !spawnInfo.player.ZoneMeteor && !spawnInfo.spiderCave;
		}

		public static bool NoZone(NPCSpawnInfo spawnInfo)
		{
			return NoZoneAllowWater(spawnInfo) && !spawnInfo.water;
		}

		public static bool NormalSpawn(NPCSpawnInfo spawnInfo)
		{
			return !spawnInfo.playerInTown && NoInvasion(spawnInfo);
		}

		public static bool NoZoneNormalSpawn(NPCSpawnInfo spawnInfo)
		{
			return NormalSpawn(spawnInfo) && NoZone(spawnInfo);
		}

		public static bool NoZoneNormalSpawnAllowWater(NPCSpawnInfo spawnInfo)
		{
			return NormalSpawn(spawnInfo) && NoZoneAllowWater(spawnInfo);
		}

		public static bool NoBiomeNormalSpawn(NPCSpawnInfo spawnInfo)
		{
			return NormalSpawn(spawnInfo) && NoBiome(spawnInfo) && NoZone(spawnInfo);
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			ExampleModMessageType msgType = (ExampleModMessageType)reader.ReadByte();
			switch (msgType)
			{
				// This message sent by the server to initialize the Volcano Tremor on clients
				case ExampleModMessageType.SetTremorTime:
					int tremorTime = reader.ReadInt32();
					ExampleWorld world = GetModWorld<ExampleWorld>();
					world.VolcanoTremorTime = tremorTime;
					break;
				// This message sent by the server to initialize the Volcano Rubble.
				case ExampleModMessageType.VolcanicRubbleMultiplayerFix:
					int numberProjectiles = reader.ReadInt32();
					for (int i = 0; i < numberProjectiles; i++)
					{
						int identity = reader.ReadInt32();
						bool found = false;
						for (int j = 0; j < 1000; j++)
						{
							if (Main.projectile[j].owner == 255 && Main.projectile[j].identity == identity && Main.projectile[j].active)
							{
								Main.projectile[j].hostile = true;
								Main.projectile[j].name = "Volcanic Rubble";
								found = true;
								break;
							}
						}
						if (!found)
						{
							ErrorLogger.Log("Error: Projectile not found");
						}
					}
					break;
				case ExampleModMessageType.SpawnNPC:
					NPC.NewNPC(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
					break;
				case ExampleModMessageType.PuritySpirit:
					PuritySpirit spirit = Main.npc[reader.ReadInt32()].modNPC as PuritySpirit;
					if (spirit != null && spirit.npc.active)
					{
						spirit.HandlePacket(reader);
					}
					break;
				case ExampleModMessageType.HeroLives:
					Player player = Main.player[reader.ReadInt32()];
					int lives = reader.ReadInt32();
					player.GetModPlayer<ExamplePlayer>(this).heroLives = lives;
					if (lives > 0)
					{
						string text = player.name + " has " + lives;
						if (lives == 1)
						{
							text += " life left!";
						}
						else
						{
							text += " lives left!";
						}
						NetMessage.SendData(25, -1, -1, text, 255, 255, 25, 25);
					}
					break;
				default:
					ErrorLogger.Log("ExampleMod: Unknown Message type: " + msgType);
					break;
			}
		}
	}

	enum ExampleModMessageType : byte
	{
		SetTremorTime,
		VolcanicRubbleMultiplayerFix,
		SpawnNPC,
		PuritySpirit,
		HeroLives
	}

	public static class ExampleModExtensions
	{
		public static int CountItem(this Player player, int type)
		{
			int count = 0;
			for (int i = 0; i < 58; i++)
			{
				if (type == player.inventory[i].type && player.inventory[i].stack > 0)
				{
					count += player.inventory[i].stack;
				}
			}
			return count;
		}
	}
}

