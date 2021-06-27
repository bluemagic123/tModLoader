﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Audio;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.Engine;
using Terraria.ModLoader.Exceptions;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.ModLoader.Utilities;

namespace Terraria.ModLoader
{
	/// <summary>
	/// Manages content added by mods.
	/// Liasons between mod content and Terraria's arrays and oversees the Loader classes.
	/// </summary>
	public static class ModContent
	{
		/// <summary> Returns the base instance of the provided content type. </summary>
		public static T GetInstance<T>() where T : class
			=> ContentInstance<T>.Instance;

		/// <summary> Returns all base content instances that derive from the provided content type across all currently loaded mods. </summary>
		public static IEnumerable<T> GetContent<T>() where T : ILoadable
			=> ModLoader.Mods.SelectMany(m => m.GetContent<T>());

		/// <summary> Attempts to find the content instance with the specified full name. Caching the result is recommended.<para/>This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		public static T Find<T>(string fullname) where T : class, IModType
			=> ModTypeLookup<T>.Get(fullname);

		/// <summary> Attempts to find the content instance with the specified name and mod name. Caching the result is recommended.<para/>This will throw exceptions on failure. </summary>
		/// <exception cref="KeyNotFoundException"/>
		public static T Find<T>(string modName, string name) where T : class, IModType
			=> ModTypeLookup<T>.Get(modName, name);

		/// <summary> Safely attempts to find the content instance with the specified full name. Caching the result is recommended. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public static bool TryFind<T>(string fullname, out T value) where T : class, IModType
			=> ModTypeLookup<T>.TryGetValue(fullname, out value);

		/// <summary> Safely attempts to find the content instance with the specified name and mod name. Caching the result is recommended. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public static bool TryFind<T>(string modName, string name, out T value) where T : class, IModType
			=> ModTypeLookup<T>.TryGetValue(modName, name, out value);

		/// <summary> Short-hand for '<see cref="GetInstance{T}"/>.Type' </summary>
		public static int GetId<T>() where T : class, IModTypeWithId
			=> GetInstance<T>().Type;

		/// <summary> Attempts to get the content instance associated with the provided integer id/type. <para/> This will throw exceptions on failure. </summary>
		/// <exception cref="IndexOutOfRangeException"/>
		/// <exception cref="KeyNotFoundException"/>
		public static T Get<T>(int id) where T : class, IModTypeWithId
			=> ModTypeLookup<T>.Get(id);

		/// <summary> Safely attempts to get the content instance associated with the provided integer id/type. </summary>
		/// <returns> Whether or not the requested instance has been found. </returns>
		public static bool TryGet<T>(int id, out T result) where T : class, IModTypeWithId
			=> ModTypeLookup<T>.TryGet(id, out result);

		private static readonly char[] nameSplitters = new char[] { '/', ' ', ':' };
		public static void SplitName(string name, out string domain, out string subName) {
			int slash = name.IndexOfAny(nameSplitters); // slash is the canonical splitter, but we'll accept space and colon for backwards compatability, just in case
			if (slash < 0)
				throw new MissingResourceException("Missing mod qualifier: " + name);

			domain = name.Substring(0, slash);
			subName = name.Substring(slash + 1);
		}

		/// <summary>
		/// Gets the byte representation of the file with the specified name. The name is in the format of "ModFolder/OtherFolders/FileNameWithExtension". Throws an ArgumentException if the file does not exist.
		/// </summary>
		/// <exception cref="MissingResourceException">Missing mod: " + name</exception>
		public static byte[] GetFileBytes(string name) {
			SplitName(name, out string modName, out string subName);

			if (!ModLoader.TryGetMod(modName, out var mod))
				throw new MissingResourceException("Missing mod: " + name);

			return mod.GetFileBytes(subName);
		}

		/// <summary>
		/// Returns whether or not a file with the specified name exists.
		/// </summary>
		public static bool FileExists(string name) {
			if (!name.Contains('/'))
				return false;

			SplitName(name, out string modName, out string subName);

			return ModLoader.TryGetMod(modName, out var mod) && mod.FileExists(subName);
		}

		/// <summary>
		/// Gets the texture with the specified name. The name is in the format of "ModFolder/OtherFolders/FileNameWithoutExtension". Throws an ArgumentException if the texture does not exist. If a vanilla texture is desired, the format "Terraria/Images/FileNameWithoutExtension" will reference an image from the "terraria/Images/Content" folder. Note: Texture2D is in the Microsoft.Xna.Framework.Graphics namespace.
		/// </summary>
		/// <exception cref="MissingResourceException">Missing mod: " + name</exception>
		public static Asset<Texture2D> GetTexture(string name) {
			if (Main.dedServ)
				return null;

			SplitName(name, out string modName, out string subName);

			if(modName == "Terraria")
				return Main.Assets.Request<Texture2D>(subName);

			if (!ModLoader.TryGetMod(modName, out var mod))
				throw new MissingResourceException($"Missing mod: {name}");

			return mod.GetTexture(subName);
		}

		/// <summary>
		/// Returns whether or not a texture with the specified name exists.
		/// </summary>
		public static bool TextureExists(string name) {
			if (Main.dedServ || string.IsNullOrWhiteSpace(name) || !name.Contains('/'))
				return false;

			SplitName(name, out string modName, out string subName);

			if (modName == "Terraria")
				return (Main.instance.Content as TMLContentManager).ImageExists(subName);

			return ModLoader.TryGetMod(modName, out var mod) && mod.TextureExists(subName);
		}

		/// <summary>
		/// Returns whether or not a texture with the specified name exists. texture will be populated with null if not found, and the texture if found.
		/// </summary>
		/// <param name="name">The texture name that is requested</param>
		/// <param name="texture">The texture itself will be output to this</param>
		/// <returns>True if the texture is found, false otherwise.</returns>
		internal static bool TryGetTexture(string name, out Asset<Texture2D> texture)
		{
			texture = null;

			if (Main.dedServ || string.IsNullOrWhiteSpace(name) || !name.Contains('/')) {
				return false;
			}

			SplitName(name, out string modName, out string subName);

			if (modName == "Terraria") {
				if ((Main.instance.Content as TMLContentManager).ImageExists(subName)) {
					texture = Main.Assets.Request<Texture2D>(subName);

					return true;
				}

				return false;
			}

			if (ModLoader.TryGetMod(modName, out var mod) && mod.TextureExists(subName)) {
				texture = mod.GetTexture(subName);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the sound with the specified name. The name is in the same format as for texture names. Throws an ArgumentException if the sound does not exist. Note: SoundEffect is in the Microsoft.Xna.Framework.Audio namespace.
		/// </summary>
		/// <exception cref="MissingResourceException">Missing mod: " + name</exception>
		public static Asset<SoundEffect> GetSound(string name) {
			if (Main.dedServ)
				return null;

			SplitName(name, out string modName, out string subName);

			if (!ModLoader.TryGetMod(modName, out var mod))
				throw new MissingResourceException("Missing mod: " + name);

			return mod.GetSound(subName);
		}

		/// <summary>
		/// Returns whether or not a sound with the specified name exists.
		/// </summary>
		public static bool SoundExists(string name) {
			if (!name.Contains('/'))
				return false;

			SplitName(name, out string modName, out string subName);

			return ModLoader.TryGetMod(modName, out var mod) && mod.SoundExists(subName);
		}

		/// <summary>
		/// Gets the music with the specified name. The name is in the same format as for texture names. Throws an ArgumentException if the music does not exist. Note: SoundMP3 is in the Terraria.ModLoader namespace.
		/// </summary>
		/// <exception cref="MissingResourceException">Missing mod: " + name</exception>
		public static Music GetMusic(string name) {
			if (Main.dedServ)
				return null;

			SplitName(name, out string modName, out string subName);

			if (!ModLoader.TryGetMod(modName, out var mod))
				throw new MissingResourceException("Missing mod: " + name);

			return mod.GetMusic(subName);
		}

		/// <summary>
		/// Returns whether or not a sound with the specified name exists.
		/// </summary>
		public static bool MusicExists(string name) {
			if (!name.Contains('/'))
				return false;

			SplitName(name, out string modName, out string subName);

			return ModLoader.TryGetMod(modName, out var mod) && mod.MusicExists(subName);
		}

		internal static void Load(CancellationToken token) {
			CacheVanillaState();

			Interface.loadMods.SetLoadStage("tModLoader.MSIntializing", ModLoader.Mods.Length);
			LoadModContent(token, mod => {
				ContentInstance.Register(mod);
				mod.loading = true;
				mod.AutoloadConfig();
				mod.PrepareAssets();
				mod.Autoload();
				mod.Load();
				SystemLoader.OnModLoad(mod);
				mod.loading = false;
			});

			Interface.loadMods.SetLoadStage("tModLoader.MSSettingUp");
			ResizeArrays();
			RecipeGroupHelper.FixRecipeGroupLookups();

			Interface.loadMods.SetLoadStage("tModLoader.MSLoading", ModLoader.Mods.Length);
			LoadModContent(token, mod => {
				mod.SetupContent();
				mod.PostSetupContent();
				SystemLoader.PostSetupContent(mod);
			});

			MemoryTracking.Finish();

			if (Main.dedServ)
				ModNet.AssignNetIDs();

			Main.player[255] = new Player();

			LocalizationLoader.RefreshModLanguage(Language.ActiveCulture);

			MapLoader.SetupModMap();
			RarityLoader.Initialize();
			
			ContentSamples.Initialize();
			PlayerInput.reinitialize = true;
			SetupBestiary(token);
			SetupRecipes(token);
			ContentSamples.RebuildItemCreativeSortingIDsAfterRecipesAreSetUp();
			ItemSorting.SetupWhiteLists();

			MenuLoader.GotoSavedModMenu();
			BossBarLoader.GotoSavedStyle();
		}
		
		private static void CacheVanillaState() {
			EffectsTracker.CacheVanillaState();
			DamageClassLoader.RegisterDefaultClasses();
			InfoDisplayLoader.RegisterDefaultDisplays();
		}

		internal static Mod LoadingMod { get; private set; }
		private static void LoadModContent(CancellationToken token, Action<Mod> loadAction) {
			MemoryTracking.Checkpoint();
			int num = 0;
			foreach (var mod in ModLoader.Mods) {
				token.ThrowIfCancellationRequested();
				Interface.loadMods.SetCurrentMod(num++, $"{mod.Name} v{mod.Version}");
				try {
					LoadingMod = mod;
					loadAction(mod);
				}
				catch (Exception e) {
					e.Data["mod"] = mod.Name;
					throw;
				}
				finally {
					LoadingMod = null;
					MemoryTracking.Update(mod.Name);
				}
			}
		}

		private static void SetupBestiary(CancellationToken token) {
			//Beastiary DB
			var bestiaryDatabase = new BestiaryDatabase();
			new BestiaryDatabaseNPCsPopulator().Populate(bestiaryDatabase);
			Main.BestiaryDB = bestiaryDatabase;
			ContentSamples.RebuildBestiarySortingIDsByBestiaryDatabaseContents(bestiaryDatabase);
			
			//Drops DB
			var itemDropDatabase = new ItemDropDatabase();
			itemDropDatabase.Populate();
			Main.ItemDropsDB = itemDropDatabase;
			
			//Update the bestiary DB with the drops DB.
			bestiaryDatabase.Merge(Main.ItemDropsDB);
			
			//Etc
			
			if (!Main.dedServ)
				Main.BestiaryUI = new UIBestiaryTest(Main.BestiaryDB);
			
			Main.ItemDropSolver = new ItemDropResolver(itemDropDatabase);
			Main.BestiaryTracker = new BestiaryUnlocksTracker();
		}

		private static void SetupRecipes(CancellationToken token) {
			Interface.loadMods.SetLoadStage("tModLoader.MSAddingRecipes");
			for (int k = 0; k < Recipe.maxRecipes; k++) {
				token.ThrowIfCancellationRequested();
				Main.recipe[k] = new Recipe();
			}

			Recipe.numRecipes = 0;
			RecipeGroupHelper.ResetRecipeGroups();
			RecipeLoader.setupRecipes = true;
			Recipe.SetupRecipes();
			RecipeLoader.setupRecipes = false;
		}

		internal static void UnloadModContent() {
			MenuLoader.Unload(); //do this early, so modded menus won't be active when unloaded
			
			int i = 0;
			
			foreach (var mod in ModLoader.Mods.Reverse()) {
				if (Main.dedServ)
					Console.WriteLine($"Unloading {mod.DisplayName}...");
				else
					Interface.loadMods.SetCurrentMod(i++, mod.DisplayName);
				
				MonoModHooks.RemoveAll(mod);
				
				try {
					mod.Close();
					mod.UnloadContent();
				}
				catch (Exception e) {
					e.Data["mod"] = mod.Name;
					throw;
				}
			}
		}

		//TODO: Unhardcode ALL of this.
		internal static void Unload() {
			ContentInstance.Clear();
			ModTypeLookup.Clear();
			ItemLoader.Unload();
			EquipLoader.Unload();
			PrefixLoader.Unload();
			DustLoader.Unload();
			TileLoader.Unload();
			WallLoader.Unload();
			ProjectileLoader.Unload();
			NPCLoader.Unload();
			NPCHeadLoader.Unload();
			BossBarLoader.Unload();
			PlayerLoader.Unload();
			BuffLoader.Unload();
			MountLoader.Unload();
			RarityLoader.Unload();
			DamageClassLoader.Unload();
			InfoDisplayLoader.Unload();
			GoreLoader.Unload();
			SoundLoader.Unload();
			DisposeMusic();

			LoaderManager.Unload();

			GlobalBackgroundStyleLoader.Unload();
			PlayerDrawLayerLoader.Unload();
			SystemLoader.Unload();
			TileEntity.manager.Reset();
			ResizeArrays(true);
			for (int k = 0; k < Recipe.maxRecipes; k++) {
				Main.recipe[k] = new Recipe();
			}
			Recipe.numRecipes = 0;
			RecipeGroupHelper.ResetRecipeGroups();
			Recipe.SetupRecipes();
			MapLoader.UnloadModMap();
			ItemSorting.SetupWhiteLists();
			RecipeLoader.Unload();
			CommandLoader.Unload();
			TagSerializer.Reload();
			ModNet.Unload();
			Config.ConfigManager.Unload();
			CustomCurrencyManager.Initialize();
			EffectsTracker.RemoveModEffects();
			
			// ItemID.Search = IdDictionary.Create<ItemID, short>();
			// NPCID.Search = IdDictionary.Create<NPCID, short>();
			// ProjectileID.Search = IdDictionary.Create<ProjectileID, short>();
			// TileID.Search = IdDictionary.Create<TileID, ushort>();
			// WallID.Search = IdDictionary.Create<WallID, ushort>();
			// BuffID.Search = IdDictionary.Create<BuffID, int>();
			
			ContentSamples.Initialize();

			CleanupModReferences();
		}

		//TODO: Unhardcode ALL of this.
		private static void ResizeArrays(bool unloading = false) {
			DamageClassLoader.ResizeArrays();
			ItemLoader.ResizeArrays(unloading);
			EquipLoader.ResizeAndFillArrays();
			PrefixLoader.ResizeArrays();
			DustLoader.ResizeArrays();
			TileLoader.ResizeArrays(unloading);
			WallLoader.ResizeArrays(unloading);
			TileIO.ResizeArrays();
			ProjectileLoader.ResizeArrays();
			NPCLoader.ResizeArrays(unloading);
			NPCHeadLoader.ResizeAndFillArrays();
			MountLoader.ResizeArrays();
			BuffLoader.ResizeArrays();
			PlayerLoader.RebuildHooks();
			PlayerDrawLayerLoader.ResizeArrays();
			SystemLoader.ResizeArrays();

			if (!Main.dedServ) {
				SoundLoader.ResizeAndFillArrays();
				GlobalBackgroundStyleLoader.ResizeAndFillArrays(unloading);
				GoreLoader.ResizeAndFillArrays();
			}

			LoaderManager.ResizeArrays();

			foreach (LocalizedText text in LanguageManager.Instance._localizedTexts.Values) {
				text.Override = null;
			}
		}

		private static void DisposeMusic() {
			//foreach (var music in Main.audioSystem.OfType<MusicStreaming>())
			//	music.Dispose();
		}

		/// <summary>
		/// Several arrays and other fields hold references to various classes from mods, we need to clean them up to give properly coded mods a chance to be completely free of references
		/// so that they can be collected by the garbage collection. For most things eventually they will be replaced during gameplay, but we want the old instance completely gone quickly.
		/// </summary>
		internal static void CleanupModReferences()
		{
			// Clear references to ModPlayer instances
			for (int i = 0; i < Main.player.Length; i++) {
				Main.player[i] = new Player();
				// player.whoAmI is only set for active players
			}

			Main.clientPlayer = new Player();
			Main.ActivePlayerFileData = new Terraria.IO.PlayerFileData();
			Main._characterSelectMenu._playerList?.Clear();
			Main.PlayerList.Clear();

			for (int i = 0; i < Main.npc.Length; i++) {
				Main.npc[i] = new NPC();
				Main.npc[i].whoAmI = i;
			}

			for (int i = 0; i < Main.item.Length; i++) {
				Main.item[i] = new Item();
				// item.whoAmI is never used
			}

			if (ItemSlot.singleSlotArray[0] != null) {
				ItemSlot.singleSlotArray[0] = new Item();
			}

			for (int i = 0; i < Main.chest.Length; i++) {
				Main.chest[i] = new Chest();
			}

			for (int i = 0; i < Main.projectile.Length; i++) {
				Main.projectile[i] = new Projectile();
				// projectile.whoAmI is only set for active projectiles
			}

			TileEntity.Clear(); // drop all possible references to mod TEs
		}

		public static Stream OpenRead(string assetName, bool newFileStream = false) {
			if (!assetName.StartsWith("tmod:"))
				return File.OpenRead(assetName);

			SplitName(assetName.Substring(5).Replace('\\', '/'), out var modName, out var entryPath);
			return ModLoader.GetMod(modName).GetFileStream(entryPath, newFileStream);
		}
	}
}
